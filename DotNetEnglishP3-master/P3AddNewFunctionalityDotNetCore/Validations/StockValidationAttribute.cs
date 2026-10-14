using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;

public class StockValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        string stockString = value as string;

        if (string.IsNullOrWhiteSpace(stockString))
            return ValidationResult.Success;

        // Récupération du localizer pour ProductViewModel
        var localizer = (IStringLocalizer<ProductViewModel>)
            validationContext.GetService(typeof(IStringLocalizer<ProductViewModel>));

        // Quantité doit être un nombre entier
        if (!int.TryParse(stockString, out int stock))
        {
            return new ValidationResult(localizer["StockNotAnInteger"]);
        }

        // Quantité doit être > 0
        if (stock <= 0)
        {
            return new ValidationResult(localizer["StockNotGreaterThanZero"]);
        }

        return ValidationResult.Success;
    }
}
