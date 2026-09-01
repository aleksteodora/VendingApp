using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Interfaces;

namespace VendingManagement.DAL.Repositories.Implementations
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        private readonly VendingDbContext _context;

        public TransactionRepository(VendingDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetByPublicIdAsync(Guid publicId)
        {
            return await _context.Transactions.FirstOrDefaultAsync(t => t.PublicId == publicId);
        }
    }
}