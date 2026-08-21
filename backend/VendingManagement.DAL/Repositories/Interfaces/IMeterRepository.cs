using VendingManagement.DAL.Entities;

namespace VendingManagement.DAL.Repositories.Interfaces
{
    public interface IMeterRepository : IRepository<Meter>
    {
        Task<Meter?> GetByUserIdAsync(int userId);
        Task<Meter?> GetBySerialNumberAsync(string serialNumber);
    }
}