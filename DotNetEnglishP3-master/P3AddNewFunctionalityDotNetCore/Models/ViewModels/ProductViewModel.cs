using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace P3AddNewFunctionalityDotNetCore.Models.ViewModels
{
    public class ProductViewModel
    {
        [BindNever]
        public int Id { get; set; }


        [Required(ErrorMessage = "MissingName")] 
        public string Name { get; set; }

        public string Description { get; set; }

        public string Details { get; set; }
        [Required(ErrorMessage = "MissingStock")]
        [StockValidation]
        public string Stock { get; set; }
        [Required(ErrorMessage = "MissingPrice")]
        [PriceValidation]
        public string Price { get; set; }
    }
}
