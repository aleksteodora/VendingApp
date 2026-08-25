using VendingManagement.DAL.Entities;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.Constants;
using Microsoft.Extensions.Logging;

namespace VendingManagement.BLL.Services.Implementations
{
    public class ProcessingFeeService : IProcessingFeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProcessingFeeService> _logger;

        public ProcessingFeeService(IUnitOfWork unitOfWork, ILogger<ProcessingFeeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ResponsePackage<ProcessingFee>> GetActiveFeeAsync()
        {
            var activeFee = await _unitOfWork.ProcessingFeeRepository.GetActiveProcessingFeeAsync();

            if (activeFee == null)
            {
                _logger.LogWarning("No active processing fee found in the database.");
                return new ResponsePackage<ProcessingFee>(
                    ResponseStatus.NotFound,
                    "No active processing fee found.");
            }

            return new ResponsePackage<ProcessingFee>(
                activeFee,
                ResponseStatus.OK,
                "Active processing fee retrieved successfully.");
        }

        public async Task<ResponsePackage<ProcessingFee>> ChangeFeeAsync(decimal fixedAmount, decimal percentageRate)
        {
            var currentActiveFees = await _unitOfWork.ProcessingFeeRepository.GetActiveProcessingFeesAsync();

            foreach (var fee in currentActiveFees)
            {
                fee.IsDeleted = true;
            }

            var newFee = new ProcessingFee
            {
                FixedAmount = fixedAmount,
                PercentageRate = percentageRate,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ProcessingFeeRepository.AddAsync(newFee);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Processing fee changed: FixedAmount={FixedAmount}, PercentageRate={PercentageRate}.", fixedAmount, percentageRate);

            return new ResponsePackage<ProcessingFee>(
                newFee,
                ResponseStatus.OK,
                "Processing fee changed successfully.");
        }
    }
}