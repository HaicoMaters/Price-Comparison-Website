using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Services.Implementations;

namespace Price_Comparison_Website.Tests.Services.Core
{
    public class PriceListingServiceTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IRepository<PriceListing>> _priceListingRepoMock;
        private readonly Mock<ILogger<PriceListingService>> _loggerMock;
        private readonly PriceListingService _priceListingService;

        public PriceListingServiceTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _priceListingRepoMock = new Mock<IRepository<PriceListing>>();
            _loggerMock = new Mock<ILogger<PriceListingService>>();

            _priceListingService = new PriceListingService(_priceListingRepoMock.Object, _loggerMock.Object,
            _notificationServiceMock.Object, _productServiceMock.Object);
        }



        // -------------------------------------------------- Update Cheapest Price ---------------------------------------------------------------------
        [Fact]
        public async Task UpdateCheapestPrice_WhenPriceIsNowLower_ShouldCreateNewPriceDropNotification()
        {
            // Arrange

            var dbProduct = new Product { ProductId = 1, CheapestPrice = 2.00m, Name = "Test" };

            _productServiceMock.Setup(r => r.GetProductById(1, It.IsAny<QueryOptions<Product>>()))
              .ReturnsAsync(dbProduct);

            // Act
            await _priceListingService.UpdateCheapestPrice(1, 1.50m);

            // Assert
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(1), Times.Once);
            _notificationServiceMock.Verify(r => r.CreateProductPriceDropNotifications(1, "Test", 1.50m, 2.00m), Times.Once);
        }

        [Fact]
        public async Task UpdateCheapestPrice_WhenPriceIsNotLower_ShouldNotCreateNewPriceDropNotification()
        {
            // Arrange
            var dbProduct = new Product { ProductId = 1, CheapestPrice = 2.00m, Name = "Test" };

            _productServiceMock.Setup(r => r.GetProductById(1, It.IsAny<QueryOptions<Product>>()))
             .ReturnsAsync(dbProduct);

            // Act
            await _priceListingService.UpdateCheapestPrice(1, 2.50m);

            // Assert
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(1), Times.Once);
            _notificationServiceMock.Verify(r => r.CreateProductPriceDropNotifications(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()
            ), Times.Never);

        }

        [Fact]
        public async Task UpdateCheapestPrice_WhenProductDoesNotExist_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dbProduct = new Product { ProductId = 1, CheapestPrice = 2.00m, Name = "Test" };

            _productServiceMock.Setup(r => r.GetProductById(1, It.IsAny<QueryOptions<Product>>()))
               .ReturnsAsync((Product)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _priceListingService.UpdateCheapestPrice(1, 2.50m)
            );

            Assert.Equal("Failed to update cheapest price", exception.Message);
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(It.IsAny<int>()), Times.Never);
        }
    }
}