using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VendingManagement.DAL.Entities
{
    [Index(nameof(MeterSerialNumber), IsUnique = true)]
    public class Meter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(13)]
        public string MeterSerialNumber { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}