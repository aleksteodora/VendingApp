using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingManagement.WebApp.Entities
{
    public class ProcessingFee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,5)")]
        public decimal FixedAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,5)")]
        public decimal PercentageRate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}