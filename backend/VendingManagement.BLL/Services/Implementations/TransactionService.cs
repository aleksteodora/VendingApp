using VendingManagement.Shared.DTOs;
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

        public async Task<TokenResponseDataOut> ProcessTransactionAsync(TokenRequestDataIn dataIn)
        {
            var meter = await _unitOfWork.Meters
                .FirstOrDefaultAsync(m => m.MeterSerialNumber == dataIn.MeterSerialNumber);

            if (meter == null)
            {
                throw new KeyNotFoundException("Meter with given serial number was not found.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(meter.UserId);

            if (user == null)
            {
                throw new KeyNotFoundException("User for given meter was not found.");
            }

            var activeFee = await _processingFeeService.GetActiveFeeAsync();

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

            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync(); //namerno je ostavljeno da se sacuva u Pending stanju
            //pre poziva security modula

            string token;
            try
            {
                token = await _securityModuleClient.RequestTokenAsync(dataIn);
            }
            catch (Exception)
            {
                transaction.Status = TransactionStatus.Failed;
                _unitOfWork.Transactions.Update(transaction);
                await _unitOfWork.SaveChangesAsync();
                throw;
            }

            transaction.Token = token;
            transaction.Status = TransactionStatus.Completed;
            _unitOfWork.Transactions.Update(transaction);
            await _unitOfWork.SaveChangesAsync(); //snimi completed

            return new TokenResponseDataOut
            {
                FullName = user.FullName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Token = token,
                EnergyAmount = energyAmount,
                ProcessingFeeAmount = processingFeeAmount
            };
        }
    }
}