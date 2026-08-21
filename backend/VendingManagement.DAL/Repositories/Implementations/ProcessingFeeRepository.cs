using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Interfaces;

namespace VendingManagement.DAL.Repositories.Implementations
{
    public class ProcessingFeeRepository : Repository<ProcessingFee>, IProcessingFeeRepository
    {
        private readonly VendingDbContext _context;

        public ProcessingFeeRepository(VendingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProcessingFee?> GetActiveProcessingFeeAsync()
        {
            return await _context.ProcessingFees
                .FirstOrDefaultAsync(x => x.IsActive);
        }

        public async Task<List<ProcessingFee>> GetActiveProcessingFeesAsync()
        {
            return await _context.ProcessingFees
                .Where(x => x.IsActive).ToListAsync();
        }
    }
}