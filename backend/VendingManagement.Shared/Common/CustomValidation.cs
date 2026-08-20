using System.ComponentModel.DataAnnotations;

namespace VendingManagement.Shared.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SerialNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var stringValue = value?.ToString();

            if (string.IsNullOrEmpty(stringValue))
            {
                return new ValidationResult("Serijski broj brojila je obavezan.");
            }

            if (stringValue.Length != 11 && stringValue.Length != 13)
            {
                return new ValidationResult("Serijski broj brojila mora imati 11 ili 13 cifara.");
            }

            if (!stringValue.All(char.IsDigit))
            {
                return new ValidationResult("Serijski broj brojila sme sadrzati samo cifre.");
            }

            return ValidationResult.Success;
        }
    }
}