using System.ComponentModel.DataAnnotations;

namespace VendingManagement.WebApp.DTOs
{
    public class UserDataIn
    {
        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(300)]
        public string Address { get; set; }

        [Required]
        [MaxLength(30)]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(13)]
        public string MeterSerialNumber { get; set; }
    }
}