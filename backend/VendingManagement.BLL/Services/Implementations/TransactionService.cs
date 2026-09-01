using VendingManagement.Shared.DTOs;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.Constants;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.BLL.Messaging;
using Microsoft.Extensions.Logging;

namespace VendingManagement.BLL.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProcessingFeeService _processingFeeService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<TransactionService> _logger;

        private const string SecurityModuleRequestQueue = "security-module-requests";

        public TransactionService(
            IUnitOfWork unitOfWork,
            IProcessingFeeService processingFeeService,
            IMessagePublisher messagePublisher,
            ILogger<TransactionService> logger)
        {
            _unitOfWork = unitOfWork;
            _processingFeeService = processingFeeService;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<ResponsePackage<TransactionAcceptedDataOut>> ProcessTransactionAsync(TokenRequestDataIn dataIn)
        {
            var meter = await _unitOfWork.MeterRepository.GetBySerialNumberAsync(dataIn.MeterSerialNumber);

            if (meter == null)
            {
                _logger.LogWarning("Transaction failed: meter with serial number {MeterSerialNumber} was not found.", dataIn.MeterSerialNumber);
                return new ResponsePackage<TransactionAcceptedDataOut>(
                    ResponseStatus.NotFound,
                    "Meter with given serial number was not found.");
            }

            var user = await _unitOfWork.CustomerRepository.GetByIdAsync(meter.UserId);

            if (user == null)
            {
                _logger.LogWarning("Transaction failed: user for meter {MeterSerialNumber} was not found.", dataIn.MeterSerialNumber);
                return new ResponsePackage<TransactionAcceptedDataOut>(
                    ResponseStatus.NotFound,
                    "User for given meter was not found.");
            }

            var feeResult = await _processingFeeService.GetActiveFeeAsync();

            if (feeResult.Status != ResponseStatus.OK)
            {
                return new ResponsePackage<TransactionAcceptedDataOut>(
                    feeResult.Status,
                    feeResult.Message);
            }

            var activeFee = feeResult.Data;

            decimal processingFeeAmount = activeFee.FixedAmount + (dataIn.Amount * activeFee.PercentageRate);
            decimal energyAmount = dataIn.Amount - processingFeeAmount;

            var transaction = new Transaction
            {
                MeterId = meter.Id,
                Amount = dataIn.Amount,
                ProcessingFeeAmount = processingFeeAmount,
                EnergyAmount = energyAmount,
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TransactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            var message = new TokenRequestMessage
            {
                TransactionId = transaction.Id,
                MeterSerialNumber = dataIn.MeterSerialNumber,
                Amount = dataIn.Amount
            };

            _messagePublisher.Publish(SecurityModuleRequestQueue, message);

            _logger.LogInformation("Transaction {TransactionId} queued for token generation, meter {MeterSerialNumber}.", transaction.Id, dataIn.MeterSerialNumber);

            var responseData = new TransactionAcceptedDataOut
            {
                TransactionId = transaction.Id,
                Status = transaction.Status.ToString(),
                Message = "Your transaction is being processed."
            };

            return new ResponsePackage<TransactionAcceptedDataOut>(
                responseData,
                ResponseStatus.Accepted,
                "Transaction accepted and queued for processing.");
        }

        public async Task HandleTokenResponseAsync(TokenResponseMessage response)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetByIdAsync(response.TransactionId);

            if (transaction == null)
            {
                _logger.LogWarning("HandleTokenResponseAsync: transaction {TransactionId} not found, skipping.", response.TransactionId);
                return;
            }

            if (response.Success)
            {
                transaction.Token = response.Token;
                transaction.Status = TransactionStatus.Completed;
                _unitOfWork.TransactionRepository.Update(transaction);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Transaction {TransactionId} completed successfully via queue.", response.TransactionId);
            }
            else
            {

                transaction.Status = TransactionStatus.Failed;
                _unitOfWork.TransactionRepository.Update(transaction);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogWarning("Transaction {TransactionId} failed: {ErrorMessage}", response.TransactionId, response.ErrorMessage);
            }
        }
    }
}