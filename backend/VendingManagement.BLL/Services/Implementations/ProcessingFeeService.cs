using VendingManagement.DAL.Entities;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.BLL.Services.Interfaces;
using VendingManagement.Shared.Common;
using VendingManagement.Shared.Constants;

namespace VendingManagement.BLL.Services.Implementations
{
    public class ProcessingFeeService : IProcessingFeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProcessingFeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponsePackage<ProcessingFee>> GetActiveFeeAsync()
        {
            var activeFee = await _unitOfWork.ProcessingFeeRepository
                .FirstOrDefaultAsync(f => f.IsActive);

            if (activeFee == null)
            {
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
                fee.IsActive = false;
                _unitOfWork.ProcessingFeeRepository.Update(fee);
            }

            var newFee = new ProcessingFee
            {
                FixedAmount = fixedAmount,
                PercentageRate = percentageRate,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ProcessingFeeRepository.AddAsync(newFee);
            await _unitOfWork.SaveChangesAsync();

            return new ResponsePackage<ProcessingFee>(
                newFee,
                ResponseStatus.OK,
                "Processing fee changed successfully.");
        }
    }
}