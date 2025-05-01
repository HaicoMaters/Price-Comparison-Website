using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PriceComparisonWebsite.Controllers;
using PriceComparisonWebsite.Models;
using PriceComparisonWebsite.Services;
using PriceComparisonWebsite.Services.HttpClients;
using PriceComparisonWebsite.Services.WebScraping.Interfaces;
using Xunit;

namespace PriceComparisonWebsite.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<ILogger<AdminController>> _loggerMock;
        private readonly Mock<ILoginActivityService> _loginActivityServiceMock;
        private readonly Mock<IAdminService> _adminServiceMock;
        private readonly Mock<IApiHttpClient> _apiHttpClientMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IScraperStatusService> _scraperStatusServiceMock;
        private readonly AdminController _adminController;

        public AdminControllerTests()
        {
            _adminServiceMock = new Mock<IAdminService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<AdminController>>();
            _loginActivityServiceMock = new Mock<ILoginActivityService>();
            _apiHttpClientMock = new Mock<IApiHttpClient>();
            _configurationMock = new Mock<IConfiguration>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _scraperStatusServiceMock = new Mock<IScraperStatusService>();

            // Setup UserManager mock
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _adminController = new AdminController(
                _adminServiceMock.Object,
                _userManagerMock.Object,
                _notificationServiceMock.Object,
                _loggerMock.Object,
                _loginActivityServiceMock.Object,
                _apiHttpClientMock.Object,
                _configurationMock.Object,
                _httpContextAccessorMock.Object,
                _scraperStatusServiceMock.Object
            );
        }

        // --------------------------------------------- Dashboard ---------------------------------------------

        [Fact]
        public async Task Dashboard_WhenNoTabSpecified_ShouldReturnNotificationsTab()
        {
            // Arrange
            var expectedTab = "notifications";
            // Act
            var result = await _adminController.Dashboard();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(expectedTab, viewResult.ViewData["ActiveTab"]);
        }

        [Fact]
        public async Task Dashboard_WhenTabSpecified_ShouldReturnCorrespondingTab()
        {
            // Arrange
            var expectedTab = "users";

            // Act
            var result = await _adminController.Dashboard(expectedTab);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(expectedTab, viewResult.ViewData["ActiveTab"]);
        }

        [Fact]
        public async Task Dashboard_ShouldSetViewBagProperties()
        {
            // Arrange
            var products = new List<Product> { new Product(), new Product() };
            var users = new List<ApplicationUser> { new ApplicationUser(), new ApplicationUser() };
            var listings = new List<PriceListing> { new PriceListing(), new PriceListing() };
            var vendors = new List<Vendor> {
                new Vendor { VendorId = 1, Name = "Vendor1", SupportsAutomaticUpdates = true },
                new Vendor { VendorId = 2, Name = "Vendor2", SupportsAutomaticUpdates = false }
            };
            var recentActivities = new List<LoginActivity> { new LoginActivity(), new LoginActivity() };
            var lastUpdate = DateTime.Now.AddHours(-1);

            _adminServiceMock.Setup(x => x.GetAllProductsAsync()).ReturnsAsync(products);
            _adminServiceMock.Setup(x => x.GetTotalUsersAsync()).ReturnsAsync(users.Count);
            _adminServiceMock.Setup(x => x.GetAllPriceListingsAsync()).ReturnsAsync(listings);
            _adminServiceMock.Setup(x => x.GetAllVendorsAsync()).ReturnsAsync(vendors);
            _loginActivityServiceMock.Setup(x => x.GetNMostRecentActivities(50)).ReturnsAsync(recentActivities);
            _scraperStatusServiceMock.Setup(x => x.GetLastUpdateTime()).ReturnsAsync(lastUpdate);

            // Act
            var result = await _adminController.Dashboard() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.ViewData["TotalProducts"]);
            Assert.Equal(2, result.ViewData["TotalUsers"]);
            Assert.Equal(2, result.ViewData["TotalListings"]);
            Assert.Equal(2, result.ViewData["TotalVendors"]);
            Assert.Equal(lastUpdate, result.ViewData["LastUpdateTime"]);
            Assert.Equal(recentActivities, result.ViewData["RecentLoginActivities"]);
            Assert.Equal("notifications", result.ViewData["ActiveTab"]);
        }

        // --------------------------------------------- UpdateAllListings ---------------------------------------------

        [Fact]
        public async Task UpdateAllListings_WhenSuccessful_ShouldReturnSuccessJson()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.NoContent);
            _apiHttpClientMock.Setup(x => x.SendAsync(HttpMethod.Patch, "api/ScraperApi/update-all-listings", null))
                .ReturnsAsync(response);

            // Act
            var result = await _adminController.UpdateAllListings() as JsonResult;

            // Assert
            Assert.NotNull(result);
            var json = JsonSerializer.Serialize(result.Value);
            var expected = JsonSerializer.Serialize(new
            {
                success = true,
                message = "Listings updated sucessfully!"
            });
            Assert.Equal(expected, json);
        }

        [Fact]
        public async Task UpdateAllListings_WhenFails_ShouldReturnErrorJson()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _apiHttpClientMock.Setup(x => x.SendAsync(HttpMethod.Patch, "api/ScraperApi/update-all-listings", null))
                .ReturnsAsync(response);

            // Act
            var result = await _adminController.UpdateAllListings() as JsonResult;

            // Assert
            Assert.NotNull(result);
            var json = JsonSerializer.Serialize(result.Value);
            var expected = JsonSerializer.Serialize(new
            {
                success = false,
                message = "Failed to update listings. Status: InternalServerError"
            });
            Assert.Equal(expected, json);
        }

        // --------------------------------------------- SendGlobalNotification ---------------------------------------------

        [Fact]
        public async Task SendGlobalNotification_WhenSuccessful_ShouldReturnSuccessJson()
        {
            // Arrange
            var message = "Test notification";
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            _apiHttpClientMock.Setup(x => x.SendAsync(HttpMethod.Post, "api/NotificationApi/create-global-notification", message))
                .ReturnsAsync(response);

            // Act
            var result = await _adminController.SendGlobalNotification(message) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var json = JsonSerializer.Serialize(result.Value);
            var expected = JsonSerializer.Serialize(new
            {
                success = true,
                message = "Global notification sent successfully!"
            });
            Assert.Equal(expected, json);
        }

        [Fact]
        public async Task SendGlobalNotification_WhenFails_ShouldReturnErrorJson()
        {
            // Arrange
            var message = "Test notification";
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _apiHttpClientMock.Setup(x => x.SendAsync(HttpMethod.Post, "api/NotificationApi/create-global-notification", message))
                .ReturnsAsync(response);

            // Act
            var result = await _adminController.SendGlobalNotification(message) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var json = JsonSerializer.Serialize(result.Value);
            var expected = JsonSerializer.Serialize(new
            {
                success = false,
                message = "Failed to send notification. Status: InternalServerError"
            });
            Assert.Equal(expected, json);
        }
    }
}