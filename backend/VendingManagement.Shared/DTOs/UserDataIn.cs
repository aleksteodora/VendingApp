using System.ComponentModel.DataAnnotations;
using VendingManagement.Shared.Common;

namespace VendingManagement.Shared.DTOs
{
    public class UserDataIn
    {
        [Required(ErrorMessage = "Ime i preyime je obavezno.")]
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Adresa je obavezna.")]
        [MaxLength(300)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Broj telefona je obavezan.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Broj telefona moze sadryati samo cifre.")]
        [MaxLength(30)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Serijski broj brojila je obavezan.")]
        [SerialNumber]
        public string MeterSerialNumber { get; set; }
    }
}