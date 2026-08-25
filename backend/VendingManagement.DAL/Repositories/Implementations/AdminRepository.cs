using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Interfaces;

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
    }
}