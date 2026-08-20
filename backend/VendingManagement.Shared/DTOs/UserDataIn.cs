using System.ComponentModel.DataAnnotations;
using VendingManagement.Shared.Common;

namespace VendingManagement.Shared.DTOs
{
    public class UserDataIn
    {
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(300)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain digits only.")]
        [MaxLength(30)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Meter serial number is required.")]
        [SerialNumber]
        public string MeterSerialNumber { get; set; }
    }
}