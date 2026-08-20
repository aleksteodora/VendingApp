using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Interfaces;

namespace VendingManagement.DAL.Repositories.Implementations
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(VendingDbContext context) : base(context)
        {
        }
    }
}