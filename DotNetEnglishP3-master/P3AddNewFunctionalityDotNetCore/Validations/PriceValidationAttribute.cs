using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

public class PriceValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Required gère déjà null ou vide
        if (value == null)
            return ValidationResult.Success;

        string priceString = value as string;

        if (string.IsNullOrWhiteSpace(priceString))
            return ValidationResult.Success;

        // Prix doit être un nombre décimal
        if (!double.TryParse(priceString, NumberStyles.Float, CultureInfo.InvariantCulture, out double price))
        {
            return new ValidationResult("PriceNotANumber");
        }

        // Prix doit être > 0
        if (price <= 0)
        {
            return new ValidationResult("PriceNotGreaterThanZero");
        }

        return ValidationResult.Success;
    }
}
