using VendingManagement.DAL.Context;
using VendingManagement.DAL.Entities;
using VendingManagement.DAL.Repositories.Interfaces;

namespace VendingManagement.DAL.Repositories.Implementations
{
    public class MeterRepository : Repository<Meter>, IMeterRepository
    {
        public MeterRepository(VendingDbContext context) : base(context)
        {
        }
    }
}