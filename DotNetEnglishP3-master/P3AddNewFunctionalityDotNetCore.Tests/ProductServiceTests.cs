using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Localization;
using NSubstitute;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
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

            // Mock du message utilisé par CheckProductModelErrors
            localizer["MissingName"].Returns(new LocalizedString("MissingName", "Veuillez saisir un nom"));
            localizer["MissingPrice"].Returns(new LocalizedString("MissingPrice", "Veuillez saisir un prix"));
            localizer["PriceNotANumber"].Returns(new LocalizedString("PriceNotANumber", "Le prix doit être un nombre valide"));
            localizer["PriceNotGreaterThanZero"].Returns(new LocalizedString("PriceNotGreaterThanZero", "Le prix doit être un nombre positif"));
            localizer["MissingStock"].Returns(new LocalizedString("MissingStock", "Veuillez saisir une quantité"));
            localizer["StockNotAnInteger"].Returns(new LocalizedString("StockNotAnInteger", "La quantité doit être un entier valide"));
            localizer["StockNotGreaterThanZero"].Returns(new LocalizedString("StockNotGreaterThanZero", "La quantité doit être un entier positif"));

            // Instanciation du ProductService avec les dépendances mockées
            _productService = new ProductService(_cart, productRepository, orderRepository, localizer);
        }

        /// <summary>
        /// Take this test method as a template to write your test method.
        /// A test method must check if a definite method does its job:
        /// returns an expected value from a particular set of parameters
        /// </summary>
        [Fact]
        public void CheckProductModelErrors_ReturnsMissingName_WhenNameIsNull()

        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = null,
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = "19.99"
            };
            // Act
            var errors = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("Veuillez saisir un nom", errors);
        }
        [Fact]
        public void CheckProductModelErrors_ReturnsMissingName_WhenNameIsWhiteSpace()

        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = " ",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = "19.99"
            };
            // Act
            var errors = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("Veuillez saisir un nom", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ReturnsMissingName_WhenNameIsEmpty()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = "19.99"
            };

            // Act
            var errors = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("Veuillez saisir un nom", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ReturnsMissingPrice_WhenPriceIsNull()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = null
            };

            // Act
            var errors = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("Veuillez saisir un prix", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ReturnsMissingPrice_WhenPriceIsWhiteSpace()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = " "
            };

            // Act
            var errors = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("Veuillez saisir un prix", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ReturnsMissingPrice_WhenPriceIsEmpty()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = ""
            };

            // Act
            var errors = _productService.CheckProductModelErrors(product);

            // Assert
            Assert.Contains("Veuillez saisir un prix", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ReturnsPriceNotANumber_WhenPriceIsNotANumber()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = "abc"
            };
            // Act
            var errors = _productService.CheckProductModelErrors(product);
            // Assert
            Assert.Contains("Le prix doit être un nombre valide", errors);

        }

        [Fact]
        public void CheckProductModelErrors_ReturnsPriceNotGreaterThanZero_WhenPriceIsZero()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = "0"
            };
            // Act
            var errors = _productService.CheckProductModelErrors(product);
            // Assert
            Assert.Contains("Le prix doit être un nombre positif", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ReturnsPriceNotGreaterThanZero_WhenPriceIsNegative()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "10",
                Price = "-5"
            };
            // Act
            var errors = _productService.CheckProductModelErrors(product);
            // Assert
            Assert.Contains("Le prix doit être un nombre positif", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ReturnsMissingStock_WhenStockIsNull()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = null,
                Price = "19.99"
            };
            // Act
            var errors = _productService.CheckProductModelErrors(product);
            // Assert
            Assert.Contains("Veuillez saisir une quantité", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ReturnsMissingStock_WhenStockIsWhiteSpace()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = " ",
                Price = "19.99"
            };
            // Act
            var errors = _productService.CheckProductModelErrors(product);
            // Assert
            Assert.Contains("Veuillez saisir une quantité", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ReturnsMissingStock_WhenStockIsEmpty()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "",
                Price = "19.99"
            };
            // Act
            var errors = _productService.CheckProductModelErrors(product);
            // Assert
            Assert.Contains("Veuillez saisir une quantité", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ReturnsStockNotAnInteger_WhenStockIsNotAnInteger()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "not_a_number",
                Price = "19.99"
            };
            // Act
            var errors = _productService.CheckProductModelErrors(product);
            // Assert
            Assert.Contains("La quantité doit être un entier valide", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ReturnsStockNotGreaterThanZero_WhenStockIsZero()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "0",
                Price = "19.99"
            };
            // Act
            var errors = _productService.CheckProductModelErrors(product);
            // Assert
            Assert.Contains("La quantité doit être un entier positif", errors);

        }

        [Fact]
        public void CheckProductModelErrors_ReturnsStockNotGreaterThanZero_WhenStockIsNegative()
        {
            // Arrange
            var product = new ProductViewModel
            {
                Name = "Valid Name",
                Description = "Valid Description",
                Details = "Valid Details",
                Stock = "-5",
                Price = "19.99"
            };
            // Act
            var errors = _productService.CheckProductModelErrors(product);
            // Assert
            Assert.Contains("La quantité doit être un entier positif", errors);
        }

        [Fact]
        public void CheckProductModelErrors_ReturnsNoErrors_WhenProductIsValid()
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
            var errors = _productService.CheckProductModelErrors(product);
            // Assert
            Assert.Empty(errors);

        }

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

    }
}