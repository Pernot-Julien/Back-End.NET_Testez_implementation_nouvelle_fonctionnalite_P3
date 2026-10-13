using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NSubstitute;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests
    {
        private readonly ProductService _productService;
        private readonly IProductRepository _productRepository;
        private readonly ICart _cart;
        private readonly IOrderRepository _orderRepository;
        private readonly IStringLocalizer<ProductService> _localizer;
        private readonly ProductController _productController;


        public ProductServiceTests()
        {
            // Mock des dépendances du ProductService
            _cart = Substitute.For<ICart>();
            var productRepository = Substitute.For<IProductRepository>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var localizer = Substitute.For<IStringLocalizer<ProductService>>();
            
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _localizer = localizer;
            _productService = new ProductService(_cart, productRepository, orderRepository, localizer);
            _productController = new ProductController(_productService, Substitute.For<ILanguageService>());
        }
        /// <summary>
        /// Vérifie que le message MissingName est renvoyé lorsque le champ Name est manquant.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ProductViewModel_ReturnsMissingName_WhenNameIsMissing(string name)
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = name,
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = "19.99"
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(product, context, results, true);

            // Assert
            Assert.Contains(results, r => r.ErrorMessage == "MissingName");
        }

        /// <summary>
        /// Vérifie que le message MissingPrice est renvoyé lorsque le champ Price est manquant.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ProductViewModel_ReturnsMissingPrice_WhenPriceIsMissing(string price)
        {
            //Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = price
            };
            //Act
            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();
            // Assert
            Validator.TryValidateObject(product, context, results, true);

            Assert.Contains(results, r => r.ErrorMessage == "MissingPrice");
        }

        /// <summary>
        /// Vérifie que le message PriceNotANumber est renvoyé lorsque le champ Price n'est pas un nombre décimal.
        /// </summary>
        [Theory]
        [InlineData("abc")]
        [InlineData("10abc")]
        [InlineData("19,99")] 
        public void ProductViewModel_ReturnsPriceNotANumber_WhenPriceIsNotDecimal(string price)
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = price
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(product, context, results, true);

            // Assert
            Assert.Contains(results, r => r.ErrorMessage == "PriceNotANumber");
        }

        /// <summary>
        /// Vérifie que le message PriceNotGreaterThanZero est renvoyé lorsque le champ Price est inférieur ou égal à zéro.
        /// </summary>
        [Theory]
        [InlineData("0")]
        [InlineData("-5")]
        public void ProductViewModel_ReturnsPriceNotGreaterThanZero_WhenPriceIsZeroOrNegative(string price)
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = price
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(product, context, results, true);

            // Assert
            Assert.Contains(results, r => r.ErrorMessage == "PriceNotGreaterThanZero");
        }



        /// <summary>
        /// Vérifie que le message MissingStock est renvoyé lorsque le champ Stock est manquant.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ProductViewModel_ReturnsMissingQuantity_WhenStockIsMissing(string stock)
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = stock,
                Price = "19.99"
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(product, context, results, true);

            // Assert
            Assert.Contains(results, r => r.ErrorMessage == "MissingStock");
        }

        /// <summary>
        /// Vérifie que le message StockNotAnInteger est renvoyé lorsque le champ Stock n'est pas un entier.
        /// </summary>
        [Theory]
        [InlineData("abc")]
        [InlineData("10.5")]
        public void ProductViewModel_ReturnsStockNotAnInteger_WhenStockIsNotInteger(string stock)
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = stock,
                Price = "19.99"
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(product, context, results, true);

            // Assert
            Assert.Contains(results, r => r.ErrorMessage == "StockNotAnInteger");
        }

        /// <summary>
        /// Vérifie que le message StockNotGreaterThanZero est renvoyé lorsque le champ Stock est inférieur ou égal à zéro.
        /// </summary>
        [Theory]
        [InlineData("0")]
        [InlineData("-5")]
        public void ProductViewModel_ReturnsStockNotGreaterThanZero_WhenStockIsZeroOrNegative(string stock)
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = stock,
                Price = "19.99"
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            // Act
            Validator.TryValidateObject(product, context, results, true);

            // Assert
            Assert.Contains(results, r => r.ErrorMessage == "StockNotGreaterThanZero");
        }


        /// <summary>
        /// Vérifie qu'aucune erreur n'est renvoyée lorsque le produit est valide.
        /// </summary>
        [Fact]
        public void ProductViewModel_ReturnsNoErrors_WhenProductIsValid()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = "19.99"
            };

            var context = new ValidationContext(product);
            var results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(product, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        /// <summary>
        /// Vérifie que la méthode SaveProduct du ProductService appelle la méthode SaveProduct du repository avec un produit valide.
        /// </summary>
        [Fact]
        public void SaveProduct_CallsRepositoryWithValidProduct()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = "19.99"
            };
            // Act
            _productService.SaveProduct(product);
            // Assert
            _productRepository.Received(1).SaveProduct(
                Arg.Is<Product>(
                    p =>
                    p.Name == product.Name &&
                    p.Description == product.Description &&
                    p.Details == product.Details &&
                    p.Quantity == int.Parse(product.Stock) &&
                    p.Price == double.Parse(product.Price, CultureInfo.InvariantCulture)
                )
             );
        }

        /// <summary>
        /// Vérifie que la méthode DeleteProduct du ProductService appelle la méthode DeleteProduct du repository avec l'ID correct et supprime le produit du panier. 
        /// </summary>
        [Fact]
        public void DeleteProduct_CallsRepositoryWithCorrectId()
        {
            // Arrange

            int id = 5;

            var product = new Product
            {
                Id = 5,
                Name = "New Product",
                Price = 10,
                Quantity = 2,
                Description = "Description",
                Details = "Details"
            };

            _productRepository.GetAllProducts().Returns(new List<Product> { product });
            // Act
            _productService.DeleteProduct(id);
            // Assert
            _cart.Received(1).RemoveLine(product);
            _productRepository.Received(1).DeleteProduct(id);
        }

        /// <summary>
        /// Vérifie que la méthode UpdateProductQuantities du ProductService appelle la méthode UpdateProductStocks du repository pour chaque produit dans le panier avec les quantités correctes.
        /// </summary>
        [Fact]
        public void UpdateProductQuantities_CallsRepositoryUpdateForEachProductInCart()
        {
            // Arrange
            var product1 = new Product { Id = 1, Name = "Product 1", Quantity = 5 };
            var product2 = new Product { Id = 2, Name = "Product 2", Quantity = 3 };

            var cart = new Cart();
            cart.AddItem(product1, 5);
            cart.AddItem(product2, 7);


            var productService = new ProductService(cart, _productRepository, _orderRepository, _localizer);

            // Act
            productService.UpdateProductQuantities();

            // Assert
            _productRepository.Received(1).UpdateProductStocks(1, 5);
            _productRepository.Received(1).UpdateProductStocks(2, 7);
        }

        /// <summary>
        /// Vérifie que lorsqu'un produit valide est créé via le ProductController, il apparaît dans la liste des produits affichée par l'action Index du contrôleur.
        /// </summary>
        [Fact]
        public void Create_ValidProduct_AppearsInClientIndex()

        {
            // Arrange
            var productViewModel = new ProductViewModel
            {
                Name = "Test Product",
                Description = "This is a test product.",
                Price = "100",
                Stock = "50",
            };

            List<Product> products = new List<Product>();

            _productRepository.GetAllProducts().Returns(_ => products);

            _productRepository
             .When(repo => repo.SaveProduct(Arg.Any<Product>()))
             .Do(call => products.Add(call.Arg<Product>()));

            // Act
            _productController.Create(productViewModel);
            var result = _productController.Index();

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            var displayedProducts = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(view.Model);
            Assert.Contains(displayedProducts, p => p.Name == productViewModel.Name);
        }
    }
}