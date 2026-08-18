using System.ComponentModel.DataAnnotations;

namespace VendingManagement.WebApp.Entities
{
    public class ProcessingFee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal FixedAmount { get; set; }

        [Required]
        public decimal PercentageRate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
