using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NuGet.ContentModel;
using Price_Comparison_Website.Services.Implementations;
using Price_Comparison_Website.Services.Interfaces;

namespace Price_Comparison_Website.Tests.Services
{
    public class ProductServiceTests
    {
        
        private readonly IProductService _productService;
        public readonly Mock<IRepository<Product>> _productRepoMock;
        public readonly Mock<IRepository<PriceListing>> _priceListingRepoMock;
        public readonly Mock<ILogger<ProductService>> _loggerMock;

        public ProductServiceTests(){
            _productRepoMock = new Mock<IRepository<Product>>();
            _priceListingRepoMock = new Mock<IRepository<PriceListing>>();
            _loggerMock = new Mock<ILogger<ProductService>>();

            _productService = new ProductService(_productRepoMock.Object, _priceListingRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task RecalculateCheapestPrice_WithPriceListings_ShouldSetCheapestPriceToThatOfCheapestListing(){
            // Arrange
            var product = new Product {ProductId = 1, CheapestPrice = 999.00m};

            var listings = new PriceListing[]{
                new PriceListing{ PriceListingId = 1, ProductId = 1, Price = 10.00m, DiscountedPrice = 10.00m},
                new PriceListing{ PriceListingId = 2, ProductId = 1, Price = 13.00m, DiscountedPrice = 9.99m}, // The price should be this listing discounted price
                new PriceListing{ PriceListingId = 3, ProductId = 1, Price = 15.32m, DiscountedPrice = 12.23m}

            };

            _productRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<Product>>())).ReturnsAsync(product);
            
            Product? capturedProduct = null;
            _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Callback<Product>(p => capturedProduct = p);  // Capture the passed to update async method

            _priceListingRepoMock.Setup(r => r.GetAllByIdAsync(1, "ProductId", It.IsAny<QueryOptions<PriceListing>>())).ReturnsAsync(listings);

            // Act
            await _productService.RecalculateCheapestPrice(1);

            // Assert
            Assert.NotNull(capturedProduct);
            Assert.Equal(9.99m, capturedProduct.CheapestPrice);
        }

        [Fact]
        public async Task RecalculateCheapestPrice_WithNoPriceListings_ShouldSetCheapestPriceToZero(){
            // Arrange
            var product = new Product {ProductId = 1, CheapestPrice = 999.00m};

            var listings = new PriceListing[]{};

            _productRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<Product>>())).ReturnsAsync(product);
            
            Product? capturedProduct = null;
            _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).Callback<Product>(p => capturedProduct = p);  // Capture the passed to update async method

            _priceListingRepoMock.Setup(r => r.GetAllByIdAsync(1, "ProductId", It.IsAny<QueryOptions<PriceListing>>())).ReturnsAsync(listings);

            // Act
            await _productService.RecalculateCheapestPrice(1);

            // Assert
            Assert.NotNull(capturedProduct);
            Assert.Equal(0.00m, capturedProduct.CheapestPrice);
        }

        [Fact]
        public async Task RecalculateCheapestPrice_WithNoProduct_ShouldThrowInvalidOperationException(){
            // Arrange
            _productRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<Product>>())).ReturnsAsync((Product)null); // Explicitly should be null
            
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _productService.RecalculateCheapestPrice(1));

            // Verify that GetByIdAsync was called once with the correct parameters
            _productRepoMock.Verify(
                r => r.GetByIdAsync(1, It.IsAny<QueryOptions<Product>>()), Times.Once);
        }
    }
}