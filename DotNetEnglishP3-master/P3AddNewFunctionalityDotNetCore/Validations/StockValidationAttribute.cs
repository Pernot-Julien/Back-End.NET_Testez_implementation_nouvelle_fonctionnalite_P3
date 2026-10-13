using System;
using System.ComponentModel.DataAnnotations;

public class StockValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Required gère déjà null ou vide
        if (value == null)
            return ValidationResult.Success;

        string stockString = value as string;

        if (string.IsNullOrWhiteSpace(stockString))
            return ValidationResult.Success;

        // Quantité doit être un nombre entier
        if (!int.TryParse(stockString, out int stock))
        {
            return new ValidationResult("StockNotAnInteger");
        }

        // Quantité doit être > 0
        if (stock <= 0)
        {
            return new ValidationResult("StockNotGreaterThanZero");
        }

        return ValidationResult.Success;
    }
}
