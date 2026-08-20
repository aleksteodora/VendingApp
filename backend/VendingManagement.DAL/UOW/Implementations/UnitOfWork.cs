using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Implementations;
using VendingManagement.DAL.Repositories.Interfaces;
using VendingManagement.DAL.UOW.Interfaces;

namespace VendingManagement.DAL.UOW.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VendingDbContext _context;

        private IRepository<User>? _userRepository;
        private IRepository<Meter>? _meterRepository;
        private IRepository<ProcessingFee>? _processingFeeRepository;
        private IRepository<Transaction>? _transactionRepository;

        public UnitOfWork(VendingDbContext context)
        {
            _context = context;
        }

        public IRepository<User> UserRepository => _userRepository ??= new Repository<User>(_context);
        public IRepository<Meter> MeterRepository => _meterRepository ??= new Repository<Meter>(_context);
        public IRepository<ProcessingFee> ProcessingFeeRepository => _processingFeeRepository ??= new Repository<ProcessingFee>(_context);
        public IRepository<Transaction> TransactionRepository => _transactionRepository ??= new Repository<Transaction>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}