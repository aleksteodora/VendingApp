using VendingManagement.DAL.Repositories.Interfaces;

namespace VendingManagement.DAL.UOW.Interfaces
{
    public interface IUnitOfWork
    {
        ICustomerRepository CustomerRepository { get; }
        IMeterRepository MeterRepository { get; }
        IProcessingFeeRepository ProcessingFeeRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        IAdminRepository AdminRepository { get; }

        Task<int> SaveChangesAsync();
    }
}