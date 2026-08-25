using VendingManagement.DAL.Entities;

namespace VendingManagement.DAL.Repositories.Interfaces
{
    public interface IAdminRepository : IRepository<Admin>
    {
        Task<Admin?> GetByEmailAsync(string email);
    }
}