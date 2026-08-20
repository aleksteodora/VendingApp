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
                return new ValidationResult("Meter serial number is required.");
            }

            if (stringValue.Length != 11 && stringValue.Length != 13)
            {
                return new ValidationResult("Meter serial number must have 11 or 13 digits.");
            }

            if (!stringValue.All(char.IsDigit))
            {
                return new ValidationResult("Meter serial number must contain digits only.");
            }

            return ValidationResult.Success;
        }
    }
}