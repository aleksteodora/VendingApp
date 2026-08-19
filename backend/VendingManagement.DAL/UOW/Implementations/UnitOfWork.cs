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

        private IRepository<User>? _users;
        private IRepository<Meter>? _meters;
        private IRepository<ProcessingFee>? _processingFees;
        private IRepository<Transaction>? _transactions;

        public UnitOfWork(VendingDbContext context)
        {
            _context = context;
        }

        public IRepository<User> Users => _users ??= new Repository<User>(_context);
        public IRepository<Meter> Meters => _meters ??= new Repository<Meter>(_context);
        public IRepository<ProcessingFee> ProcessingFees => _processingFees ??= new Repository<ProcessingFee>(_context);
        public IRepository<Transaction> Transactions => _transactions ??= new Repository<Transaction>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}