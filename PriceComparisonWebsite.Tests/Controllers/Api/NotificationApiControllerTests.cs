using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PriceComparisonWebsite.Controllers.Api;
using PriceComparisonWebsite.Models;
using PriceComparisonWebsite.Services;
using Xunit;

namespace PriceComparisonWebsite.Tests.Controllers.Api
{
    public class NotificationApiControllerTests
    {
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<ILogger<NotificationApiController>> _loggerMock;
        private readonly NotificationApiController _notificationApiController;

        public NotificationApiControllerTests()
        {
            _notificationServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<NotificationApiController>>();
            _notificationApiController = new NotificationApiController(_notificationServiceMock.Object, _loggerMock.Object);
        }

        // --------------------------------------------- CreateGlobalNotification ---------------------------------------------
        [Fact]
        public async Task CreateGlobalNotification_WithValidMessage_ShouldSendGlobalNotification()
        {
            // Arrange
            string message = "Message";

            // Act
            var result = await _notificationApiController.CreateGlobalNotification(message);

            // Assert
            _notificationServiceMock.Verify(r => r.CreateGlobalNotification(message), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task CreateGlobalNotification_WithEmptyMessage_ShouldReturnBadRequest()
        {
            // Arrange
            string message = "";

            // Act
            var result = await _notificationApiController.CreateGlobalNotification(message);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task CreateGlobalNotification_WhenExceptionThrown_ShouldReturn500()
        {
            // Arrange
            string message = "Test Message";
            _notificationServiceMock.Setup(x => x.CreateGlobalNotification(message))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _notificationApiController.CreateGlobalNotification(message);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // --------------------------------------------- GetUserNotifications ---------------------------------------------

        [Fact]
        public async Task GetUserNotifications_WithBothReadAndUnreadNotifications_ShouldReturnCombinedNotifications()
        {
            // Arrange
            var userId = "user123";
            var readNotifications = new List<Notification>
            {
                new Notification { Id = 1, Message = "Read1", CreatedAt = DateTime.Now.AddHours(-1) }
            };
            var unreadNotifications = new List<Notification>
            {
                new Notification { Id = 2, Message = "Unread1", CreatedAt = DateTime.Now }
            };

            _notificationServiceMock.Setup(x => x.GetReadUserNotifications(userId))
                .ReturnsAsync(readNotifications);
            _notificationServiceMock.Setup(x => x.GetUnreadUserNotifications(userId))
                .ReturnsAsync(unreadNotifications);

            // Act
            var result = await _notificationApiController.GetUserNotifications(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetUserNotifications_WhenExceptionThrown_ShouldReturn500()
        {
            // Arrange
            var userId = "user123";
            _notificationServiceMock.Setup(x => x.GetReadUserNotifications(userId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _notificationApiController.GetUserNotifications(userId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // --------------------------------------------- MarkNotificationsAsRead ---------------------------------------------

        [Fact]
        public async Task MarkNotificationsAsRead_ShouldCallService()
        {
            // Arrange
            var userId = "user123";

            // Act
            var result = await _notificationApiController.MarkNotificationsAsRead(userId);

            // Assert
            _notificationServiceMock.Verify(x => x.MarkNotificationsAsRead(userId), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task MarkNotificationsAsRead_WhenExceptionThrown_ShouldReturn500()
        {
            // Arrange
            var userId = "user123";
            _notificationServiceMock.Setup(x => x.MarkNotificationsAsRead(userId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _notificationApiController.MarkNotificationsAsRead(userId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // --------------------------------------------- DismissNotification ---------------------------------------------

        [Fact]
        public async Task DismissNotification_WithNotificationId_ShouldCallService()
        {
            // Arrange
            var userId = "user123";
            var notificationId = 1;

            // Act
            var result = await _notificationApiController.DismissNotification(userId, notificationId);

            // Assert
            _notificationServiceMock.Verify(x => x.DeleteUserNotification(notificationId, userId), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task DismissNotification_WhenExceptionThrown_ShouldReturn500()
        {
            // Arrange
            var userId = "user123";
            var notificationId = 1;
            _notificationServiceMock.Setup(x => x.DeleteUserNotification(notificationId, userId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _notificationApiController.DismissNotification(userId, notificationId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}