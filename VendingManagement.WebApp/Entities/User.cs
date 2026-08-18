using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace VendingManagement.WebApp.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

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
        [MaxLength(100)]
        public string ApiKey { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Meter Meter { get; set; }
    }
}