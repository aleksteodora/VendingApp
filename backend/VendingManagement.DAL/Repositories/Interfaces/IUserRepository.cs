using VendingManagement.DAL.Entities;

namespace VendingManagement.DAL.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<(List<User> Users, int TotalCount)> GetPagedWithMeterAsync(int pageNumber, int pageSize);
        Task<bool> ExistsByApiKeyAsync(string apiKey);
    }
}