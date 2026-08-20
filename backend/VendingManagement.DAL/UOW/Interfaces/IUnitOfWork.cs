using VendingManagement.DAL.Repositories.Interfaces;

namespace VendingManagement.DAL.UOW.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMeterRepository MeterRepository { get; }
        IProcessingFeeRepository ProcessingFeeRepository { get; }
        ITransactionRepository TransactionRepository { get; }

        Task<int> SaveChangesAsync();
    }
}