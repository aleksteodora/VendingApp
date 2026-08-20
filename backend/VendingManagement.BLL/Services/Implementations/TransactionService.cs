using VendingManagement.Shared.DTOs;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.Constants;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.BLL.Clients;

namespace VendingManagement.BLL.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProcessingFeeService _processingFeeService;
        private readonly ISecurityModuleClient _securityModuleClient;

        public TransactionService(
            IUnitOfWork unitOfWork,
            IProcessingFeeService processingFeeService,
            ISecurityModuleClient securityModuleClient)
        {
            _unitOfWork = unitOfWork;
            _processingFeeService = processingFeeService;
            _securityModuleClient = securityModuleClient;
        }

        public async Task<ResponsePackage<TokenResponseDataOut>> ProcessTransactionAsync(TokenRequestDataIn dataIn)
        {
            var meter = await _unitOfWork.MeterRepository
                .FirstOrDefaultAsync(m => m.MeterSerialNumber == dataIn.MeterSerialNumber);

            if (meter == null)
            {
                return new ResponsePackage<TokenResponseDataOut>(
                    ResponseStatus.NotFound,
                    "Meter with given serial number was not faund.");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(meter.UserId);

            if (user == null)
            {
                return new ResponsePackage<TokenResponseDataOut>(
                    ResponseStatus.NotFound,
                    "User for given meter was not found.");
            }

            var feeResult = await _processingFeeService.GetActiveFeeAsync();

            if (feeResult.Status != ResponseStatus.OK)
            {
                return new ResponsePackage<TokenResponseDataOut>(
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

            string token;
            try
            {
                token = await _securityModuleClient.RequestTokenAsync(dataIn);
            }
            catch (Exception ex)
            {
                transaction.Status = TransactionStatus.Failed;
                _unitOfWork.TransactionRepository.Update(transaction);
                await _unitOfWork.SaveChangesAsync();

                return new ResponsePackage<TokenResponseDataOut>(
                    ResponseStatus.InternalServerError,
                    $"Failed to generate token: {ex.Message}");
            }

            transaction.Token = token;
            transaction.Status = TransactionStatus.Completed;
            _unitOfWork.TransactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync();

            var responseData = new TokenResponseDataOut
            {
                FullName = user.FullName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Token = token,
                EnergyAmount = energyAmount,
                ProcessingFeeAmount = processingFeeAmount
            };

            return new ResponsePackage<TokenResponseDataOut>(
                responseData,
                ResponseStatus.OK,
                "Transaction processed successfully.");
        }
    }
}