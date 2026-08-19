using VendingManagement.DAL.Entities;

namespace VendingManagement.BLL.Services.Interfaces
{
    public interface IProcessingFeeService
    {
        Task<ProcessingFee> GetActiveFeeAsync();
        Task<ProcessingFee> ChangeFeeAsync(decimal fixedAmount, decimal percentageRate);
    }
}
