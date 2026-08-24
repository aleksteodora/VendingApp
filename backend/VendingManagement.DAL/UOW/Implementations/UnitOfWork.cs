using VendingManagement.DAL.Context;
using VendingManagement.DAL.Repositories.Implementations;
using VendingManagement.DAL.Repositories.Interfaces;
using VendingManagement.DAL.UOW.Interfaces;

namespace VendingManagement.DAL.UOW.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VendingDbContext _context;

        private ICustomerRepository? _customerRepository;
        private IMeterRepository? _meterRepository;
        private IProcessingFeeRepository? _processingFeeRepository;
        private ITransactionRepository? _transactionRepository;

        public UnitOfWork(VendingDbContext context)
        {
            _context = context;
        }

        public ICustomerRepository CustomerRepository => _customerRepository ??= new CustomerRepository(_context);
        public IMeterRepository MeterRepository => _meterRepository ??= new MeterRepository(_context);
        public IProcessingFeeRepository ProcessingFeeRepository => _processingFeeRepository ??= new ProcessingFeeRepository(_context);
        public ITransactionRepository TransactionRepository => _transactionRepository ??= new TransactionRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}