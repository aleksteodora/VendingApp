using System.ComponentModel.DataAnnotations;
using VendingManagement.Shared.Common;

namespace VendingManagement.Shared.DTOs
{
    public class TokenRequestDataIn
    {
        [Required(ErrorMessage = "Meter serial number is required.")]
        [SerialNumber]
        public string MeterSerialNumber { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.00001, 999999999999.99999, ErrorMessage = "Amount must be a positive value.")]
        public decimal Amount { get; set; }
    }
}