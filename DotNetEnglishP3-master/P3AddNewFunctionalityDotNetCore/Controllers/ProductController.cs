using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace P3AddNewFunctionalityDotNetCore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILanguageService _languageService;
        private readonly IStringLocalizer<ProductService> _localizer;


        public ProductController(IProductService productService, ILanguageService languageService, IStringLocalizer<ProductService> localizer)
        {
            _productService = productService;
            _languageService = languageService;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            IEnumerable<ProductViewModel> products = _productService.GetAllProductsViewModel();
            return View(products);
        }

        [Authorize]
        public IActionResult Admin()
        {
            return View(_productService.GetAllProductsViewModel().OrderByDescending(p => p.Id));
        }

        [Authorize]
        public ViewResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(ProductViewModel product)
        {
            // Required
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // Validation métier
            var errors = _productService.CheckProductModelErrors(product);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    if (error.Contains("Price"))
                    {
                        ModelState.AddModelError(nameof(product.Price), _localizer[error]);
                    }

                    if (error.Contains("Stock"))
                    {
                        ModelState.AddModelError(nameof(product.Stock), _localizer[error]);
                    }
                }

                return View(product);
            }

            // Save
            _productService.SaveProduct(product);

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            _productService.DeleteProduct(id);
            return RedirectToAction("Admin");
        }
    }
}