using VendingManagement.DAL.Entities;

namespace VendingManagement.DAL.Repositories.Interfaces
{
    public interface IProcessingFeeRepository : IRepository<ProcessingFee>
    {
        Task<List<ProcessingFee>> GetActiveProcessingFeesAsync();
    }
}