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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique ApiKey
            modelBuilder.Entity<User>()
                .HasIndex(u => u.ApiKey)
                .IsUnique();

            // Unique MeterSerialNumber
            modelBuilder.Entity<Meter>()
                .HasIndex(m => m.MeterSerialNumber)
                .IsUnique();

            // 1-1
            modelBuilder.Entity<Meter>()
                .HasOne(m => m.User)
                .WithOne(u => u.Meter)
                .HasForeignKey<Meter>(m => m.UserId);

            // brojilo moze imati vise transakcija
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Meter)
                .WithMany()
                .HasForeignKey(t => t.MeterId);

            // dec
            modelBuilder.Entity<ProcessingFee>()
                .Property(p => p.FixedAmount)
                .HasPrecision(18, 5);

            modelBuilder.Entity<ProcessingFee>()
                .Property(p => p.PercentageRate)
                .HasPrecision(18, 5);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 5);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.ProcessingFeeAmount)
                .HasPrecision(18, 5);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.EnergyAmount)
                .HasPrecision(18, 5);
        }
    }
}
