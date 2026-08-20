using System.ComponentModel.DataAnnotations;

namespace VendingManagement.Shared.DTOs
{
    public class ProcessingFeeDataIn
    {
        [Required(ErrorMessage = "Fixed amount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Fixed amount cannot be negative.")]
        public decimal FixedAmount { get; set; }

        [Required(ErrorMessage = "Percentage rate is required.")]
        [Range(0, 1, ErrorMessage = "Percentage rate must be between 0 and 1.")]
        public decimal PercentageRate { get; set; }
    }
}