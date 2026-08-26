using VendingManagement.DAL.Entities;
using VendingManagement.Shared.Common;

namespace VendingManagement.BLL.Services.Interfaces
{
    public interface IProcessingFeeService
    {
        Task<ResponsePackage<ProcessingFee>> GetActiveFeeAsync();
        Task<ResponsePackage<ProcessingFee>> ChangeFeeAsync(decimal fixedAmount, decimal percentageRate);
        Task<ResponsePackage<List<ProcessingFee>>> GetHistoryAsync();
    }
}