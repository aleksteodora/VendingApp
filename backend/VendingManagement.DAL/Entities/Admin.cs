using System.ComponentModel.DataAnnotations;
using VendingManagement.Shared.Constants;

namespace VendingManagement.DAL.Entities
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; }

        public AdminRole Role { get; set; } = AdminRole.Admin;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}