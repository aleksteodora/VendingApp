using Microsoft.EntityFrameworkCore;
using VendingManagement.DAL.Entities;

namespace VendingManagement.DAL.Context
{
    public class VendingDbContext : DbContext
    {
        public VendingDbContext(DbContextOptions<VendingDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<ProcessingFee> ProcessingFees { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Meter>()
                .HasIndex(m => m.MeterSerialNumber)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = false");
        }
    }
}