using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Interfaces;

namespace VendingManagement.DAL.Repositories.Implementations
{
    public class ProcessingFeeRepository : Repository<ProcessingFee>, IProcessingFeeRepository
    {
        public ProcessingFeeRepository(VendingDbContext context) : base(context)
        {
        }

        public async Task<List<ProcessingFee>> GetActiveProcessingFeesAsync()
        {
            return await _context.ProcessingFees
                .Where(x => x.IsActive).ToListAsync();
        }
    }
}