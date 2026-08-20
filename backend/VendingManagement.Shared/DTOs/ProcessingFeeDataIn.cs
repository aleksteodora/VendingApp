using System.ComponentModel.DataAnnotations;

namespace VendingManagement.Shared.DTOs
{
    public class ProcessingFeeDataIn
    {
        [Required(ErrorMessage = "Fiksni iznos je obavezan.")]
        [Range(0, double.MaxValue, ErrorMessage = "Fiksni iznos ne sme biti negativan.")]
        public decimal FixedAmount { get; set; }

        [Required(ErrorMessage = "Procenat je obavezan.")]
        [Range(0, 1, ErrorMessage = "Procenat mora biti izmedju 0 i 1.")]
        public decimal PercentageRate { get; set; }
    }
}