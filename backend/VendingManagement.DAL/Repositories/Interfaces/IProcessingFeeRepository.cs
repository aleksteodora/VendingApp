using VendingManagement.DAL.Entities;

namespace VendingManagement.DAL.Repositories.Interfaces
{
    public interface IProcessingFeeRepository : IRepository<ProcessingFee>
    {
        Task<ProcessingFee?> GetActiveProcessingFeeAsync();
        Task<List<ProcessingFee>> GetActiveProcessingFeesAsync();
    }
}