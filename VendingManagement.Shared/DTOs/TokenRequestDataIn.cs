
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
        [Range(-999999999999.99999, 999999999999.99999, ErrorMessage = "The Amount field must have no more than 17 digits in total, with 5 decimal places.")]
        public decimal Amount { get; set; }
    }
}
