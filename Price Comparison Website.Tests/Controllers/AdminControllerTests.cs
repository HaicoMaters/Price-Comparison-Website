using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Price_Comparison_Website.Services.WebScraping.Interfaces;

namespace Price_Comparison_Website.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<ILogger<AdminController>> _loggerMock;
        private readonly Mock<ILoginActivityService> _loginActivityServiceMock;
        private readonly Mock<IAdminService> _adminServiceMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
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

            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _configurationMock = new Mock<IConfiguration>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _scraperStatusServiceMock = new Mock<IScraperStatusService>();

            // Mocking UserManager<ApplicationUser>
            var store = new Mock<IUserStore<ApplicationUser>>();

            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            _adminController = new AdminController(
                _adminServiceMock.Object,
                _userManagerMock.Object,
                _notificationServiceMock.Object,
                _loggerMock.Object,
                _loginActivityServiceMock.Object,
                _httpClientFactoryMock.Object,
                _configurationMock.Object,
                _httpContextAccessorMock.Object,
                _scraperStatusServiceMock.Object
            );
        }


        // ------------------------------------------------- SendGlobalNotification -----------------------------------------

        [Fact]
        public async Task SendGlobalNotification_WithValidMessage_ShouldSendGlobalNotification()
        {
            // Arrange
            string message = "Message";

            // Act
            _adminController.SendGlobalNotification(message);

            // Assert
            _notificationServiceMock.Verify(r => r.CreateGlobalNotification(message), Times.Once);
        }

        [Fact]
        public async Task SendGlobalNotification_WithValidMessage_ShouldReturnRedirectToDashboardWithSuccessMessage()
        {
            // Arrange
            string message = "Message";

            // Mock TempData
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _adminController.TempData = tempData;

            // Act
            var result = await _adminController.SendGlobalNotification(message) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dashboard", result.ActionName);
            Assert.True(tempData.ContainsKey("SuccessMessage"));
            Assert.Equal("Global notification sent successfully!", tempData["SuccessMessage"]);

            _notificationServiceMock.Verify(r => r.CreateGlobalNotification(message), Times.Once);
        }



        [Fact]
        public async Task SendGlobalNotification_WithNullMessage_ShouldReturnRedirectToDashboardWithModelError()
        {
            // Arrange
            string message = null;

            // Act
            var result = await _adminController.SendGlobalNotification(message) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dashboard", result.ActionName);

            // Verify that ModelState contains the expected error message
            Assert.False(_adminController.ModelState.IsValid);
            Assert.True(_adminController.ModelState.ContainsKey(""));
            Assert.Contains("Message cannot be empty", _adminController.ModelState[""].Errors.Select(e => e.ErrorMessage));

            _notificationServiceMock.Verify(r => r.CreateGlobalNotification(It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public async Task SendGlobalNotification_WithOnlyWhiteSpace_ShouldReturnRedirectToDashboardWithModelError()
        {
            // Arrange
            string message = "       ";

            // Act
            var result = await _adminController.SendGlobalNotification(message) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dashboard", result.ActionName);

            // Verify that ModelState contains the expected error message
            Assert.False(_adminController.ModelState.IsValid);
            Assert.True(_adminController.ModelState.ContainsKey(""));
            Assert.Contains("Message cannot be empty", _adminController.ModelState[""].Errors.Select(e => e.ErrorMessage));

            _notificationServiceMock.Verify(r => r.CreateGlobalNotification(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SendGlobalNotification_WhenExceptionOccurs_ShouldRedirectToDashboardWithErrorMessage()
        {
            // Arrange
            string message = "Message";

            _notificationServiceMock.Setup(r => r.CreateGlobalNotification(message))
            .ThrowsAsync(new Exception("Test Exception"));

            // Mock TempData
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _adminController.TempData = tempData;

            // Act
            var result = await _adminController.SendGlobalNotification(message) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dashboard", result.ActionName);
            Assert.True(tempData.ContainsKey("ErrorMessage"));
            Assert.Equal("Error sending notification: Test Exception", tempData["ErrorMessage"]);

            _notificationServiceMock.Verify(r => r.CreateGlobalNotification(message), Times.Once);
        }

        // ------------------------------------------------        ------------------------------------------------------
    }
}