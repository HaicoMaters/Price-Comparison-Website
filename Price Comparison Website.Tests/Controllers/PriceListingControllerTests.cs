using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using Price_Comparison_Website.Services;
using Price_Comparison_Website.Services.Interfaces;
using Xunit.Sdk;

namespace Price_Comparison_Website.Tests.Controllers
{
    public class PriceListingControllerTests
    {
        private readonly Mock<IPriceListingService> _priceListingServiceMock;
        private readonly Mock<IVendorService> _vendorServiceMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<ILogger<PriceListingController>> _loggerMock;
        private readonly PriceListingController _priceListingController;

        public PriceListingControllerTests()
        {
            _priceListingServiceMock = new Mock<IPriceListingService>();
            _productServiceMock = new Mock<IProductService>();
            _vendorServiceMock = new Mock<IVendorService>();
            _loggerMock = new Mock<ILogger<PriceListingController>>();

            _priceListingController = new PriceListingController(_priceListingServiceMock.Object, _vendorServiceMock.Object,
            _productServiceMock.Object, _loggerMock.Object);
        }

        // --------------------------------------------------- AddEdit [HTTP GET] ----------------------------------------------------------
        [Fact]
        public async Task AddEditGet_WhenIdIs0_ShouldReturnAddView()
        {
            // Arrange
            int id = 0;
            int prodId = 1;

            var product = new Product { ProductId = prodId, Name = "Test Product" };
            var vendors = new List<Vendor> { new Vendor { VendorId = 1, Name = "Test Vendor" } };

            _productServiceMock.Setup(s => s.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(product);

            _vendorServiceMock.Setup(s => s.GetAllVendorsAsync())
                .ReturnsAsync(vendors);

            // Act
            var result = await _priceListingController.AddEdit(id, prodId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Add", result.ViewData["Operation"]);
            Assert.Equal(product, result.ViewData["Product"]);
            Assert.Equal(vendors, result.ViewData["Vendors"]);
            var model = Assert.IsType<PriceListing>(result.Model);
            Assert.Equal(prodId, model.ProductId);
        }

        [Fact]
        public async Task AddEditGet_WhenIdIsNot0_ShouldReturnEditView()
        {
            // Arrange
            int id = 1;
            int prodId = 1;

            var product = new Product { ProductId = prodId, Name = "Test Product" };
            var priceListing = new PriceListing { PriceListingId = id, ProductId = prodId, Price = 100m };
            var vendors = new List<Vendor> { new Vendor { VendorId = 1, Name = "Test Vendor" } };

            _productServiceMock.Setup(s => s.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(product);

            _priceListingServiceMock.Setup(s => s.GetPriceListingById(id, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(priceListing);

            _vendorServiceMock.Setup(s => s.GetAllVendorsAsync())
                .ReturnsAsync(vendors);

            // Act
            var result = await _priceListingController.AddEdit(id, prodId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Edit", result.ViewData["Operation"]);
            Assert.Equal(product, result.ViewData["Product"]);
            Assert.Equal(vendors, result.ViewData["Vendors"]);
            var model = Assert.IsType<PriceListing>(result.Model);
            Assert.Equal(id, model.PriceListingId);
            Assert.Equal(prodId, model.ProductId);
        }

        [Fact]
        public async Task AddEditGet_WhenProductNotFound_ShouldReturnNotFound()
        {
            // Arrange
            int id = 1;
            int prodId = 100;  // Non-existent product

            _productServiceMock.Setup(s => s.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
            .ReturnsAsync((Product)null);  // Simulate product not found

            // Act
            var result = await _priceListingController.AddEdit(id, prodId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _priceListingServiceMock.Verify(p => p.GetPriceListingById(It.IsAny<int>(), It.IsAny<QueryOptions<PriceListing>>()), Times.Never);
        }

        [Fact]
        public async Task AddEditGet_WhenPriceListingNotFound_ShouldReturnNotFound()
        {
            // Arrange
            int id = 1;
            int prodId = 1;

            var product = new Product { ProductId = prodId, Name = "Test Product" };

            _productServiceMock.Setup(s => s.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(product);

            _priceListingServiceMock.Setup(s => s.GetPriceListingById(id, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync((PriceListing)null); // Price listing not found

            // Act
            var result = await _priceListingController.AddEdit(id, prodId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _priceListingServiceMock.Verify(p => p.GetPriceListingById(It.IsAny<int>(), It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
        }

        [Fact]
        public async Task AddEditGet_WhenExceptionThrown_ShouldReturnStatusCode500()
        {
            // Arrange
            int id = 1;
            int prodId = 1;

            _productServiceMock.Setup(s => s.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _priceListingController.AddEdit(id, prodId) as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }

        // ---------------------------------------------- AddEdit [HTTP POST] -------------------------------------------------------------------
        [Fact]
        public async Task AddEditPost_WhenPriceListingIdIs0_ShouldAddNewPriceListing()
        { // This Can be made better by moving UpdateCheapestPrice() into service
            // Arrange
            decimal originalDiscountedPrice = 1.99m;
            var listing = new PriceListing { PriceListingId = 0, ProductId = 1, Price = 2.00m, DiscountedPrice = originalDiscountedPrice }; // IsDiscounted is not checked so the added discounted price should = price
            var product = new Product { ProductId = 1, CheapestPrice = 1.00m, Name = "Test" }; // Cheaper than listing added (not testing UpdateCheapestPrice())

            _productServiceMock.Setup(r => r.GetProductById(1, It.IsAny<QueryOptions<Product>>())).ReturnsAsync(product); // For the update cheapest price call

            // Mock HttpContext and Form
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = formCollection;

            _priceListingController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };


            // Act
            await _priceListingController.AddEdit(listing);

            // Assert

            Assert.Equal(listing.Price, listing.DiscountedPrice); // Check Discount Logic is correct
            Assert.NotEqual(originalDiscountedPrice, listing.DiscountedPrice);

            // Function call verification
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(1), Times.Once);
            _priceListingServiceMock.Verify(r => r.AddPriceListing(It.IsAny<PriceListing>()), Times.Once);
        }

        [Fact]
        public async Task AddEditPost_WhenPriceListingIdIsNot0_ShouldUpdatePriceListing()
        {
            // Arrange
            decimal originalDiscountedPrice = 1.99m;
            var listing = new PriceListing { PriceListingId = 1, ProductId = 1, Price = 2.00m, DiscountedPrice = originalDiscountedPrice }; // IsDiscounted is not checked so the added discounted price should = price
            var product = new Product { ProductId = 1, CheapestPrice = 1.00m, Name = "Test" }; // Cheaper than listing added (not testing UpdateCheapestPrice())

            _productServiceMock.Setup(r => r.GetProductById(1, It.IsAny<QueryOptions<Product>>())).ReturnsAsync(product); // For the update cheapest price call

            // Mock HttpContext and Form
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = formCollection;

            _priceListingController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };


            // Act
            await _priceListingController.AddEdit(listing);

            // Assert

            Assert.Equal(listing.Price, listing.DiscountedPrice); // Check Discount Logic is correct
            Assert.NotEqual(originalDiscountedPrice, listing.DiscountedPrice);

            // Function call verification
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(1), Times.Once);
            _priceListingServiceMock.Verify(r => r.UpdatePriceListing(It.IsAny<PriceListing>()), Times.Once);
        }

        [Fact]
        public async Task AddEditPost_WhenSucessful_ShouldRedirectToViewProductPageOfListing()
        { // Does not matter if add or edit operation used just doing add in this case
            // Arrange

            decimal originalDiscountedPrice = 1.99m;

            var listing = new PriceListing { PriceListingId = 0, ProductId = 1, Price = 2.00m, DiscountedPrice = originalDiscountedPrice }; // IsDiscounted is not checked so the added discounted price should = price
            var product = new Product { ProductId = 1, CheapestPrice = 1.00m, Name = "Test" }; 

            _productServiceMock.Setup(r => r.GetProductById(1, It.IsAny<QueryOptions<Product>>())).ReturnsAsync(product); // For the update cheapest price call

            // Mock HttpContext and Form
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = formCollection;

            _priceListingController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };


            // Act
            var result = await _priceListingController.AddEdit(listing);

            // Assert

            // Discount logic checks
            Assert.Equal(listing.Price, listing.DiscountedPrice); // Check Discount Logic is correct
            Assert.NotEqual(originalDiscountedPrice, listing.DiscountedPrice);

            // Redirect checks
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewProduct", redirectResult.ActionName);
            Assert.Equal("Product", redirectResult.ControllerName);
            Assert.Equal(listing.ProductId, redirectResult.RouteValues["id"]);

            // Function call verification
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(1), Times.Once);
            _priceListingServiceMock.Verify(r => r.AddPriceListing(It.IsAny<PriceListing>()), Times.Once);
        }

        [Fact]
        public async Task AddEditPost_WhenIsDiscounted_ShouldUpdateDiscountedPrice()
        { // Does not matter if add or edit operation used just doing edit in this case
            // Arrange
            decimal originalDiscountedPrice = 1.50m;

            var listing = new PriceListing { PriceListingId = 1, ProductId = 1, Price = 2.00m, DiscountedPrice = originalDiscountedPrice }; // IsDiscounted is not checked so the added discounted price should = price
            var product = new Product { ProductId = 1, CheapestPrice = 1.00m, Name = "Test" }; // Cheaper than listing added (not testing UpdateCheapestPrice())

            _productServiceMock.Setup(r => r.GetProductById(1, It.IsAny<QueryOptions<Product>>())).ReturnsAsync(product); // For the update cheapest price call

            // Mock HttpContext with "IsDiscounted" in Form
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "IsDiscounted", "true" }
            });

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = formCollection;

            _priceListingController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };


            // Act
            await _priceListingController.AddEdit(listing);

            // Assert

            Assert.Equal(originalDiscountedPrice, listing.DiscountedPrice); // Check Discount Logic is correct
            Assert.NotEqual(listing.Price, listing.DiscountedPrice);
            Assert.NotEqual(originalDiscountedPrice, listing.Price);

            // Function call verification
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(1), Times.Once);
            _priceListingServiceMock.Verify(r => r.UpdatePriceListing(It.IsAny<PriceListing>()), Times.Once);
        }

        [Fact]
        public async Task AddEditPost_WhenIsDiscountedWithMoreExpensiveDiscount_ShouldNotDiscountedPrice()
        { // Does not matter if add or edit operation used just doing edit in this case
            // Arrange
            decimal originalDiscountedPrice = 2.50m;

            var listing = new PriceListing { PriceListingId = 1, ProductId = 1, Price = 2.00m, DiscountedPrice = originalDiscountedPrice }; // IsDiscounted is not checked so the added discounted price should = price
            var product = new Product { ProductId = 1, CheapestPrice = 1.00m, Name = "Test" }; // Cheaper than listing added (not testing UpdateCheapestPrice())

            _productServiceMock.Setup(r => r.GetProductById(1, It.IsAny<QueryOptions<Product>>())).ReturnsAsync(product); // For the update cheapest price call

            // Mock HttpContext with "IsDiscounted" in Form
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "IsDiscounted", "true" }
            });

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = formCollection;

            _priceListingController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };


            // Act
            await _priceListingController.AddEdit(listing);

            // Assert

            Assert.NotEqual(originalDiscountedPrice, listing.DiscountedPrice); // Check Discount Logic is correct
            Assert.Equal(listing.Price, listing.DiscountedPrice);
            Assert.NotEqual(originalDiscountedPrice, listing.Price);

            // Function call verification
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(1), Times.Once);
            _priceListingServiceMock.Verify(r => r.UpdatePriceListing(It.IsAny<PriceListing>()), Times.Once);
        }

        [Fact]
        public async Task AddEditPost_WhenModelStateIsInvalid_ShouldReturnTheView()
        {
            // Arrange
            var listing = new PriceListing { PriceListingId = 1, ProductId = 1, Price = 2.00m, DiscountedPrice = 1.99m };

            _priceListingController.ModelState.AddModelError("Price", "Invalid price");

            // Act
            var result = await _priceListingController.AddEdit(listing) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(listing, result.Model);  // Ensure the original listing is returned
            Assert.False(result.ViewData.ModelState.IsValid);

            // Verify that no service calls were made since it should exit early
            _priceListingServiceMock.Verify(r => r.AddPriceListing(It.IsAny<PriceListing>()), Times.Never);
            _priceListingServiceMock.Verify(r => r.UpdatePriceListing(It.IsAny<PriceListing>()), Times.Never);
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(It.IsAny<int>()), Times.Never);
            _loggerMock.VerifyNoOtherCalls();
        }


        [Fact]
        public async Task AddEditPost_WhenInvalidOperationExceptionThrown_ShouldReturnBadRequest()
        {
            // Arrange
            var listing = new PriceListing { PriceListingId = 1, ProductId = 1, Price = 2.00m, DiscountedPrice = 1.99m };

            _productServiceMock.Setup(r => r.RecalculateCheapestPrice(It.IsAny<int>()))
            .ThrowsAsync(new InvalidOperationException("Test exception")); // For the update cheapest price call (should throw exception)
            
            // Mock HttpContext and Form
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = formCollection;

            _priceListingController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _priceListingController.AddEdit(listing);

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            // Verify no other services are called after the exception
            _priceListingServiceMock.Verify(r => r.AddPriceListing(It.IsAny<PriceListing>()), Times.Never);
            _priceListingServiceMock.Verify(r => r.UpdatePriceListing(It.IsAny<PriceListing>()), Times.Never);
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(It.IsAny<int>()), Times.Once);
        }


        [Fact]
        public async Task AddEditPost_WhenOtherExceptionsThrown_ShouldReturnStatusCode500()
        {
            // Arrange
            var listing = new PriceListing { PriceListingId = 1, ProductId = 1, Price = 2.00m, DiscountedPrice = 1.99m };

            _productServiceMock.Setup(r => r.RecalculateCheapestPrice(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Test exception")); // For the update cheapest price call (should throw exception)
            
            // Mock HttpContext and Form
            var formCollection = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = formCollection;

            _priceListingController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _priceListingController.AddEdit(listing) as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);

            // Verify no other services are called after the exception
            _priceListingServiceMock.Verify(r => r.AddPriceListing(It.IsAny<PriceListing>()), Times.Never);
            _priceListingServiceMock.Verify(r => r.UpdatePriceListing(It.IsAny<PriceListing>()), Times.Never);
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(It.IsAny<int>()), Times.Once);
        }

        // ---------------------------------------------------- Delete ------------------------------------
        [Fact]
        public async Task Delete_WhenPricelistingExists_ShouldDeleteListing(){
            // Arrange
            var priceListing = new PriceListing{PriceListingId = 1, ProductId = 1};

            _priceListingServiceMock.Setup(r => r.GetPriceListingById(1, It.IsAny<QueryOptions<PriceListing>>()))
            .ReturnsAsync(priceListing);

            // Act
            await _priceListingController.Delete(1);

            // Assert
            _priceListingServiceMock.Verify(r => r.DeletePriceListing(1), Times.Once);
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(1), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenPricelistingExists_ShouldReturnViewProductViewWithSameProductId(){
            // Arrange
            var priceListing = new PriceListing{PriceListingId = 1, ProductId = 1};

            _priceListingServiceMock.Setup(r => r.GetPriceListingById(1, It.IsAny<QueryOptions<PriceListing>>()))
            .ReturnsAsync(priceListing);

            // Act
            var result = await _priceListingController.Delete(1);

            // Assert
            _priceListingServiceMock.Verify(r =>r.GetPriceListingById(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(r => r.DeletePriceListing(1), Times.Once);
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(1), Times.Once);

            // Redirect checks
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewProduct", redirectResult.ActionName);
            Assert.Equal("Product", redirectResult.ControllerName);
            Assert.Equal(priceListing.ProductId, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Delete_WhenNullPricelisting_ShouldReturnNotFound(){
            // Arrange
            _priceListingServiceMock.Setup(r => r.GetPriceListingById(1, It.IsAny<QueryOptions<PriceListing>>()))
            .ReturnsAsync((PriceListing)null);

            // Act
            var result = await _priceListingController.Delete(1);

            // Assert
            _priceListingServiceMock.Verify(r =>r.GetPriceListingById(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(r => r.DeletePriceListing(1), Times.Never);
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(1), Times.Never);

            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            
        }

        [Fact]
        public async Task Delete_WhenInvalidOperationException_ShouldReturnBadRequest(){
             // Arrange
            var priceListing = new PriceListing{PriceListingId = 1, ProductId = 1};

            _priceListingServiceMock.Setup(r => r.GetPriceListingById(1, It.IsAny<QueryOptions<PriceListing>>()))
            .ReturnsAsync(priceListing);

            _priceListingServiceMock.Setup(r => r.DeletePriceListing(1))
            .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act
            var result = await _priceListingController.Delete(1);

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            _priceListingServiceMock.Verify(r =>r.GetPriceListingById(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(r => r.DeletePriceListing(1), Times.Once);
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(1), Times.Never);
        }   

        [Fact]
        public async Task Delete_WhenOtherExceptions_ShouldReturnErrorCode500(){
            // Arrange
            _priceListingServiceMock.Setup(r => r.GetPriceListingById(1, It.IsAny<QueryOptions<PriceListing>>()))
            .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _priceListingController.Delete(1) as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            
            _priceListingServiceMock.Verify(r =>r.GetPriceListingById(1, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
            _priceListingServiceMock.Verify(r => r.DeletePriceListing(1), Times.Never);
            _productServiceMock.Verify(r => r.RecalculateCheapestPrice(1), Times.Never);
        }
    }
}