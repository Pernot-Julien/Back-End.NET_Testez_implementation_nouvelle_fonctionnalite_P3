using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using NSubstitute;
using P3AddNewFunctionalityDotNetCore.Controllers;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductIntegrationTests
    {
        private readonly IProductRepository _productRepository;
        private readonly ILanguageService _languageService;
        private readonly ProductController _productController;
        private readonly IProductService _productService;
        private readonly ICart _cart;
        private readonly IOrderRepository _orderRepository;
        private readonly IStringLocalizer<ProductService> _localizer;



        public ProductIntegrationTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _languageService = Substitute.For<ILanguageService>();
            _cart = Substitute.For<ICart>();
            _orderRepository = Substitute.For<IOrderRepository>();
            _localizer = Substitute.For<IStringLocalizer<ProductService>>();
            _productService = new ProductService(_cart, _productRepository, _orderRepository, _localizer);
            _productController = new ProductController(_productService, _languageService);

        }

        [Fact]
        public void Create_ValidProduct_PersistsInDatabase_VisibleInClientList()

        {
            // Arrange
            var testConnectionString = @"Server=localhost\SQLEXPRESS;Database=P3Referential_Test;Trusted_Connection=True;MultipleActiveResultSets=true";
            P3Referential P3Referential = CreateTestContext(testConnectionString);
            P3Referential.Database.Migrate();

            ProductRepository productRepository = new ProductRepository(P3Referential);
            Cart cart = new Cart();
            OrderRepository orderRepository = new OrderRepository(P3Referential);
            ProductService productService = new ProductService(cart, productRepository, orderRepository, _localizer);

            // Act
            ProductController productController = new ProductController(productService, _languageService);
            ProductViewModel productViewModel = new ProductViewModel
            {
                Name = "MacBook Pro",
                Description = "This is a test product.",
                Price = "100",
                Stock = "50",
            };
            try
            {
                var result = productController.Create(productViewModel);
                using var verificationContext = CreateTestContext(testConnectionString);
                var savedProduct = verificationContext.Set<Product>().FirstOrDefault(p => p.Name == productViewModel.Name);
                // Assert  
                Assert.NotNull(savedProduct);
                var indexResult = productController.Index();
                var viewResult = Assert.IsType<ViewResult>(indexResult);
                var displayedProducts = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(viewResult.ViewData.Model);
                Assert.Contains(displayedProducts, p => p.Name == productViewModel.Name);
            }
            finally
            {
                using var cleanupContext = CreateTestContext(testConnectionString);
                var productToRemove = cleanupContext.Set<Product>().FirstOrDefault(p => p.Name == productViewModel.Name);

                if (productToRemove != null)
                {
                    cleanupContext.Remove(productToRemove);
                    cleanupContext.SaveChanges();
                }
            }
        }
        [Fact]
        public void Remove_ExistingProduct_RemovesFromDatabaseAndClientList()

        {
            // Arrange
            var testConnectionString = @"Server=localhost\SQLEXPRESS;Database=P3Referential_Test;Trusted_Connection=True;MultipleActiveResultSets=true";

         

            P3Referential P3Referential = CreateTestContext(testConnectionString);
            P3Referential.Database.Migrate();
            ProductRepository productRepository = new ProductRepository(P3Referential);
            Cart cart = new Cart();
            OrderRepository orderRepository = new OrderRepository(P3Referential);
            ProductService productService = new ProductService(cart, productRepository, orderRepository, _localizer);

            // Act
            ProductController productController = new ProductController(productService, _languageService);
            ProductViewModel productViewModel = new ProductViewModel
            {
                Name = "MacBook Air 512go",
                Description = "This is a second test product.",
                Price = "1500",
                Stock = "12"
            };
            try
            {
                var result = productController.Create(productViewModel);
                using var verificationContext = CreateTestContext(testConnectionString);
                var savedProduct = verificationContext.Set<Product>().FirstOrDefault(p => p.Name == productViewModel.Name);

                // Assert
                Assert.NotNull(savedProduct);
                var deleteResult = productController.DeleteProduct(savedProduct.Id);
                P3Referential P3ReferentialDelete = CreateTestContext(testConnectionString);
                Assert.DoesNotContain(P3ReferentialDelete.Set<Product>(), p => p.Id == savedProduct.Id);
                var indexResult = productController.Index();
                var viewResult = Assert.IsType<ViewResult>(indexResult);
                var displayedProducts = Assert.IsAssignableFrom<IEnumerable<ProductViewModel>>(viewResult.ViewData.Model);
                Assert.DoesNotContain(displayedProducts, p => p.Name == productViewModel.Name);
            }
            finally
            {
                using var cleanupContext = CreateTestContext(testConnectionString);
                var productToRemove = cleanupContext.Set<Product>().FirstOrDefault(p => p.Name == productViewModel.Name);

                if (productToRemove != null)
                {
                    cleanupContext.Remove(productToRemove);
                    cleanupContext.SaveChanges();
                }
            }
        }

        private P3Referential CreateTestContext(string testConnectionString)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:P3Referential"] = testConnectionString
                })
                .Build();
            var options = new DbContextOptionsBuilder<P3Referential>()
                .UseSqlServer(testConnectionString)
                .Options;
            var p3ReferentialTest = new P3Referential(options, config);
            return p3ReferentialTest;
        }
    }
}