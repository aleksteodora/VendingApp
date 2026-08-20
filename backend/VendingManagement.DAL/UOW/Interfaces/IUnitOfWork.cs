using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Interfaces;

namespace VendingManagement.DAL.UOW.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<User> UserRepository { get; }
        IRepository<Meter> MeterRepository { get; }
        IRepository<ProcessingFee> ProcessingFeeRepository { get; }
        IRepository<Transaction> TransactionRepository { get; }

        Task<int> SaveChangesAsync();
    }
}