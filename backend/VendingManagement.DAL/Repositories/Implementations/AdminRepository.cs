using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Interfaces;
using VendingManagement.Shared.Constants;

namespace VendingManagement.DAL.Repositories.Implementations
{
    public class AdminRepository : Repository<Admin>, IAdminRepository
    {
        private readonly VendingDbContext _context;

        public AdminRepository(VendingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<(List<AdminListItem> Admins, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Admins
                .AsNoTracking()
                .Where(a => !a.IsDeleted && a.Role != AdminRole.SuperAdmin);

            var totalCount = await query.CountAsync();

            var admins = await query
                .OrderBy(a => a.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AdminListItem
                {
                    Id = a.Id,
                    Email = a.Email,
                    FullName = a.FullName
                })
                .ToListAsync();

            return (admins, totalCount);
        }
    }
}