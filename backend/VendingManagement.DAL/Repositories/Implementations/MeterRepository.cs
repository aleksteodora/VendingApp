using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Interfaces;

namespace VendingManagement.DAL.Repositories.Implementations
{
    public class MeterRepository : Repository<Meter>, IMeterRepository
    {
        private readonly VendingDbContext _context;

        public MeterRepository(VendingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Meter?> GetByUserIdAsync(int userId)
        {
            return await _context.Meters
                .FirstOrDefaultAsync(m => m.UserId == userId);
        }

        public async Task<Meter?> GetBySerialNumberAsync(string serialNumber)
        {
            return await _context.Meters
                .FirstOrDefaultAsync(m => m.MeterSerialNumber == serialNumber);
        }
    }
}