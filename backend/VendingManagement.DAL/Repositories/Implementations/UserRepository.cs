using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Interfaces;

namespace VendingManagement.DAL.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly VendingDbContext _context;

        public UserRepository(VendingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<User> Users, int TotalCount)> GetPagedWithMeterAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _context.Users.CountAsync();

            var users = await _context.Users
                .Include(u => u.Meter)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<bool> ExistsByApiKeyAsync(string apiKey)
        {
            return await _context.Users.AnyAsync(u => u.ApiKey == apiKey);
        }
    }
}