using System.ComponentModel.DataAnnotations;
using VendingManagement.Shared.Common;

namespace VendingManagement.Shared.DTOs
{
    public class TokenRequestDataIn
    {
        [Required(ErrorMessage = "Serijski broj brojila je obavezan.")]
        [SerialNumber]
        public string MeterSerialNumber { get; set; }

        [Required(ErrorMessage = "Iznos je obavezan.")]
        [Range(0.00001, 999999999999.99999, ErrorMessage = "Iynos mora biti poyitivna vrednost.")]
        public decimal Amount { get; set; }
    }
}