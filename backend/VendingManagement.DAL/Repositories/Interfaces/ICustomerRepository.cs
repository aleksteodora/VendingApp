using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories;

namespace VendingManagement.DAL.Repositories.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<(List<CustomerListItem> Users, int TotalCount)> GetPagedWithMeterAsync(int pageNumber, int pageSize);
        Task<Customer?> GetByIdWithMeterAsync(int id);
        Task<bool> ExistsByApiKeyAsync(string apiKey);

        Task<Customer?> GetByApiKeyAsync(string apiKey);
    }
}