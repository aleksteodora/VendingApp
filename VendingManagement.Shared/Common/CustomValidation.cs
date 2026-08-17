using System.ComponentModel.DataAnnotations;

namespace VendingManagement.Shared.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SerialNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value != null && (value.ToString()?.Length == 11 || value.ToString()?.Length == 13))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Meter serial number must have 11 or 13 digits.");
        }
    }

}
