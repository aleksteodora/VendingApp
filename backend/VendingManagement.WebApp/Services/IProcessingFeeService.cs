using VendingManagement.WebApp.Entities;

namespace VendingManagement.WebApp.Services
{
    public interface IProcessingFeeService
    {
        Task<ProcessingFee> GetActiveFeeAsync();
        Task<ProcessingFee> ChangeFeeAsync(decimal fixedAmount, decimal percentageRate);
    }
}
