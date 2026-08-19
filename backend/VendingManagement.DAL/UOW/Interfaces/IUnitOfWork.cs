using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Interfaces;

namespace VendingManagement.DAL.UOW.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<User> Users { get; }
        IRepository<Meter> Meters { get; }
        IRepository<ProcessingFee> ProcessingFees { get; }
        IRepository<Transaction> Transactions { get; }

        Task<int> SaveChangesAsync();
    }
}