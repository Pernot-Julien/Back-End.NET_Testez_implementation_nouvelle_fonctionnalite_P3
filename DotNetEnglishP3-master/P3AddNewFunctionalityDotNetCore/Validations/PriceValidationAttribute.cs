using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

public class PriceValidationAttribute : ValidationAttribute
{
    public PriceValidationAttribute()
    {
        ErrorMessage = "PriceNotANumber"; // valeur par défaut
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        string priceString = value as string;

        if (string.IsNullOrWhiteSpace(priceString))
            return ValidationResult.Success;

        // Localizer pour ProductViewModel
        var localizer = (IStringLocalizer<ProductViewModel>)
            validationContext.GetService(typeof(IStringLocalizer<ProductViewModel>));

        // Non numérique
        if (!double.TryParse(priceString, NumberStyles.Float, CultureInfo.InvariantCulture, out double price))
        {
            return new ValidationResult(localizer["PriceNotANumber"]);
        }

        // Prix <= 0
        if (price <= 0)
        {
            return new ValidationResult(localizer["PriceNotGreaterThanZero"]);
        }

        return ValidationResult.Success;
    }
}
