using VendingManagement.DAL.Entities;

namespace VendingManagement.DAL.Repositories.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<(List<Customer> Users, int TotalCount)> GetPagedWithMeterAsync(int pageNumber, int pageSize);
        Task<Customer?> GetByIdWithMeterAsync(int id);
        Task<bool> ExistsByApiKeyAsync(string apiKey);
    }
}