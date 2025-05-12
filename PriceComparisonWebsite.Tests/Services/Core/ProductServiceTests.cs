using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NuGet.ContentModel;
using PriceComparisonWebsite.Services.Implementations;
using PriceComparisonWebsite.Services.Interfaces;

namespace PriceComparisonWebsite.Tests.Services
{
    public class ProductServiceTests
    {

        private readonly IProductService _productService;
        public readonly Mock<IRepository<Product>> _productRepoMock;
        public readonly Mock<IRepository<PriceListing>> _priceListingRepoMock;
        private readonly Mock<IRepository<ProductPriceHistory>> _priceHistoryRepoMock;
        public readonly Mock<ILogger<ProductService>> _loggerMock;

        public ProductServiceTests()
        {
            _productRepoMock = new Mock<IRepository<Product>>();
            _priceListingRepoMock = new Mock<IRepository<PriceListing>>();
            _priceHistoryRepoMock = new Mock<IRepository<ProductPriceHistory>>();
            _loggerMock = new Mock<ILogger<ProductService>>();

            _productService = new ProductService(
                _productRepoMock.Object, 
                _priceListingRepoMock.Object,
                _priceHistoryRepoMock.Object,
                _loggerMock.Object);
        }

        // ---------------------------------------- Recalculate Cheapest Price -----------------------------------------------

        [Fact]
        public async Task RecalculateCheapestPrice_WithPriceListings_ShouldSetCheapestPriceToThatOfCheapestListing()
        {
            // Arrange
            var product = new Product { ProductId = 1, CheapestPrice = 999.00m };

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
        public async Task RecalculateCheapestPrice_WithNoPriceListings_ShouldSetCheapestPriceToZero()
        {
            // Arrange
            var product = new Product { ProductId = 1, CheapestPrice = 999.00m };

            var listings = new PriceListing[] { };

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
        public async Task RecalculateCheapestPrice_WithNoProduct_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _productRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<Product>>())).ReturnsAsync((Product)null); // Explicitly should be null

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _productService.RecalculateCheapestPrice(1));

            // Verify that GetByIdAsync was called once with the correct parameters
            _productRepoMock.Verify(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<Product>>()), Times.Once);
        }

        // ---------------------------------------- Record Price History -----------------------------------------------

        [Fact]
        public async Task RecordPriceHistory_ShouldAddNewHistoryEntry()
        {
            // Arrange
            int productId = 1;
            decimal price = 10.99m;
            ProductPriceHistory? capturedHistory = null;

            _priceHistoryRepoMock.Setup(r => r.AddAsync(It.IsAny<ProductPriceHistory>()))
                .Callback<ProductPriceHistory>(h => capturedHistory = h);

            // Act
            await _productService.RecordPriceHistory(productId, price);

            // Assert
            Assert.NotNull(capturedHistory);
            Assert.Equal(productId, capturedHistory.ProductId);
            Assert.Equal(price, capturedHistory.Price);
            Assert.True((DateTime.Now - capturedHistory.Timestamp).TotalSeconds < 5); // 5 seconds tolerance

            _priceHistoryRepoMock.Verify(r => r.AddAsync(It.IsAny<ProductPriceHistory>()), Times.Once);
        }

        // ---------------------------------------- Get Price History -----------------------------------------------

        [Fact]
        public async Task GetPriceHistory_ShouldReturnOrderedHistory()
        {
            // Arrange
            int productId = 1;
            var history = new List<ProductPriceHistory>
            {
                new ProductPriceHistory { ProductId = productId, Price = 10.99m, Timestamp = DateTime.Now.AddDays(-2) },
                new ProductPriceHistory { ProductId = productId, Price = 9.99m, Timestamp = DateTime.Now.AddDays(-1) }
            };

            _priceHistoryRepoMock.Setup(r => r.GetAllAsync(It.IsAny<QueryOptions<ProductPriceHistory>>()))
                .ReturnsAsync(history);

            // Act
            var result = await _productService.GetPriceHistory(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(history, result);

            _priceHistoryRepoMock.Verify(r => r.GetAllAsync(It.Is<QueryOptions<ProductPriceHistory>>(
                opt => opt.OrderBy != null && opt.Where != null
            )), Times.Once);
        }

        [Fact]
        public async Task RecalculateCheapestPrice_WhenPriceChanges_ShouldRecordNewHistoryEntry()
        {
            // Arrange
            var product = new Product { ProductId = 1, CheapestPrice = 10.99m };
            var listings = new[] {
                new PriceListing { ProductId = 1, Price = 9.99m, DiscountedPrice = 9.99m }
            };

            _productRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(product);
            _priceListingRepoMock.Setup(r => r.GetAllByIdAsync(1, "ProductId", It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listings);

            ProductPriceHistory? capturedHistory = null;
            _priceHistoryRepoMock.Setup(r => r.AddAsync(It.IsAny<ProductPriceHistory>()))
                .Callback<ProductPriceHistory>(h => capturedHistory = h);

            // Act
            await _productService.RecalculateCheapestPrice(1);

            // Assert
            Assert.NotNull(capturedHistory);
            Assert.Equal(1, capturedHistory.ProductId);
            Assert.Equal(9.99m, capturedHistory.Price);

            _priceHistoryRepoMock.Verify(r => r.AddAsync(It.IsAny<ProductPriceHistory>()), Times.Once);
        }

        [Fact]
        public async Task RecalculateCheapestPrice_WhenPriceUnchanged_ShouldNotRecordNewHistoryEntry()
        {
            // Arrange
            var product = new Product { ProductId = 1, CheapestPrice = 9.99m };
            var listings = new[] {
                new PriceListing { ProductId = 1, Price = 9.99m, DiscountedPrice = 9.99m }
            };

            _productRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(product);
            _priceListingRepoMock.Setup(r => r.GetAllByIdAsync(1, "ProductId", It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listings);

            // Act
            await _productService.RecalculateCheapestPrice(1);

            // Assert
            _priceHistoryRepoMock.Verify(r => r.AddAsync(It.IsAny<ProductPriceHistory>()), Times.Never);
        }

        // ---------------------------------------- Setup Pagination -----------------------------------------------

        [Fact]
        public void SetupPagination_WithLessThan12ProductAndPage1_ShouldReturnListOfAllProducts()
        {
            // Arrange
            var products = Enumerable.Range(1, 10)
                .Select(i => new Product { ProductId = i })
                .ToList();

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            // Act
            var pagedProducts = _productService.SetupPagination(products, 1, viewData);

            // Assert
            Assert.NotNull(pagedProducts);
            Assert.Equal(10, pagedProducts.Count);

            Assert.Equal(Enumerable.Range(1, 10), pagedProducts.Select(p => p.ProductId));
        }


        [Fact]
        public void SetupPagination_WithMoreThan12ProductsAndPage1_ShouldReturnListOfFirst12()
        {
            // Arrange
            var products = Enumerable.Range(1, 46)
                .Select(i => new Product { ProductId = i })
                .ToList();

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            // Act
            var pagedProducts = _productService.SetupPagination(products, 1, viewData);

            // Assert
            Assert.NotNull(pagedProducts);
            Assert.Equal(12, pagedProducts.Count);

            Assert.Equal(Enumerable.Range(1, 12), pagedProducts.Select(p => p.ProductId));
        }


        [Fact]
        public void SetupPagination_WithMoreThan12ProductsAndPage2_ShouldReturnListOfProductAfterTheFirst12()
        {
            // Arrange
            var products = Enumerable.Range(1, 46)
                .Select(i => new Product { ProductId = i })
                .ToList();

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            // Act
            var pagedProducts = _productService.SetupPagination(products, 4, viewData);

            // Assert
            Assert.NotNull(pagedProducts);
            Assert.Equal(10, pagedProducts.Count);

            Assert.Equal(Enumerable.Range(37, 10), pagedProducts.Select(p => p.ProductId));
        }


        [Fact]
        public void SetupPagination_ShouldSetupViewDataCorrectly()
        {
            // Arrange
            var products = Enumerable.Range(1, 46)
                .Select(i => new Product { ProductId = i })
                .ToList();

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            // Act
            var pagedProducts = _productService.SetupPagination(products, 2, viewData);

            // Assert
            Assert.Equal(2, viewData["PageNumber"]);
            Assert.Equal(4, viewData["TotalPages"]);

            Assert.NotNull(pagedProducts);
            Assert.Equal(12, pagedProducts.Count);

            Assert.Equal(Enumerable.Range(13, 12), pagedProducts.Select(p => p.ProductId));
        }
    }
}