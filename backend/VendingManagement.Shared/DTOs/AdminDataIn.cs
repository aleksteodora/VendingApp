using System.ComponentModel.DataAnnotations;

namespace VendingManagement.Shared.DTOs
{
    public class AdminDataIn
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(200)]
        public string FullName { get; set; }
    }
}