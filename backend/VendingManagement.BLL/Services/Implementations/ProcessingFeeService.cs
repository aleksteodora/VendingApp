using VendingManagement.DAL.Entities;
using VendingManagement.DAL.UOW.Interfaces;
using VendingManagement.BLL.Services.Interfaces;

namespace VendingManagement.BLL.Services.Implementations
{
    public class ProcessingFeeService : IProcessingFeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProcessingFeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProcessingFee> GetActiveFeeAsync()
        {
            var activeFee = await _unitOfWork.ProcessingFeeRepository
                .FirstOrDefaultAsync(f => f.IsActive);

            if (activeFee == null)
            {
                throw new InvalidOperationException("No active processing fee found.");
            }

            return activeFee;
        }

        public async Task<ProcessingFee> ChangeFeeAsync(decimal fixedAmount, decimal percentageRate)
        {
            var currentActiveFees = await _unitOfWork.ProcessingFeeRepository
                .FindAsync(f => f.IsActive);

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

            return newFee;
        }
    }
}