using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Interfaces;

namespace VendingManagement.DAL.Repositories.Implementations
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        private readonly VendingDbContext _context;

        public CustomerRepository(VendingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<Customer> Users, int TotalCount)> GetPagedWithMeterAsync(int pageNumber, int pageSize)
        {
            var query = _context.Customers.AsNoTracking().Where(u => !u.IsDeleted);

            var totalCount = await query.CountAsync();

            var users = await query
                .Include(u => u.Meter)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<Customer?> GetByIdWithMeterAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Meter)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByApiKeyAsync(string apiKey)
        {
            return await _context.Customers.AnyAsync(u => u.ApiKey == apiKey);
        }
    }
}