using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VendingManagement.DAL.Entities
{
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
        public Customer User { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}