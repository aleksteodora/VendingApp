using System.ComponentModel.DataAnnotations;

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

        public bool IsSuperAdmin { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}