using Microsoft.EntityFrameworkCore;
using VendingManagement.WebApp.Entities;

namespace VendingManagement.WebApp.Data
{
    public class VendingDbContext : DbContext
    {
        public VendingDbContext(DbContextOptions<VendingDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<ProcessingFee> ProcessingFees { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}