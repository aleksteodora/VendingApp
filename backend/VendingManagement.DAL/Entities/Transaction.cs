using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingManagement.DAL.Entities
{
    public enum TransactionStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2
    }

    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MeterId { get; set; }

        [ForeignKey(nameof(MeterId))]
        public Meter Meter { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,5)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,5)")]
        public decimal ProcessingFeeAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,5)")]
        public decimal EnergyAmount { get; set; }

        public string? Token { get; set; }

        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}