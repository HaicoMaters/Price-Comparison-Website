using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PriceComparisonWebsite.Controllers.Api;
using PriceComparisonWebsite.Services.WebScraping.Interfaces;
using Xunit;

namespace PriceComparisonWebsite.Tests.Controllers.Api
{
    public class ScraperApiControllerTests
    {
        private readonly Mock<IPriceScraperService> _priceScraperServiceMock;
        private readonly Mock<ILogger<ScraperApiController>> _loggerMock;
        private readonly ScraperApiController _scraperApiController;

        public ScraperApiControllerTests()
        {
            _priceScraperServiceMock = new Mock<IPriceScraperService>();
            _loggerMock = new Mock<ILogger<ScraperApiController>>();
            _scraperApiController = new ScraperApiController(_priceScraperServiceMock.Object, _loggerMock.Object);
        }

        // --------------------------------------------- UpdateAllListings ---------------------------------------------

        [Fact]
        public async Task UpdateAllListings_WhenSuccessful_ShouldReturnNoContent()
        {
            // Arrange
            _priceScraperServiceMock.Setup(x => x.UpdateAllListings())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _scraperApiController.UpdateAllListings();

            // Assert
            _priceScraperServiceMock.Verify(x => x.UpdateAllListings(), Times.Once);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, ((NoContentResult)result).StatusCode);
        }

        [Fact]
        public async Task UpdateAllListings_WhenExceptionThrown_ShouldReturn500()
        {
            // Arrange
            _priceScraperServiceMock.Setup(x => x.UpdateAllListings())
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _scraperApiController.UpdateAllListings();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}