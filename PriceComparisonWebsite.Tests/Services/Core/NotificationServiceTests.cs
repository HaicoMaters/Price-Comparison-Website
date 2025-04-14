using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PriceComparisonWebsite.Services.Interfaces;

namespace PriceComparisonWebsite.Tests.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<IRepository<UserNotification>> _userNotficationRepoMock;
        private readonly Mock<IRepository<Notification>> _notificationRepoMock;
        private readonly Mock<IRepository<ApplicationUser>> _applicationUserRepoMock;
        private readonly Mock<IRepository<UserWishList>> _userWishListRepoMock;
        private readonly INotificationService _notificationService;
        private readonly Mock<ILogger<NotificationService>> _loggerMock;

        public NotificationServiceTests()
        {
            _applicationUserRepoMock = new Mock<IRepository<ApplicationUser>>();
            _notificationRepoMock = new Mock<IRepository<Notification>>();
            _userNotficationRepoMock = new Mock<IRepository<UserNotification>>();
            _userWishListRepoMock = new Mock<IRepository<UserWishList>>();
            _loggerMock = new Mock<ILogger<NotificationService>>();

            _notificationService = new NotificationService(_userNotficationRepoMock.Object, _notificationRepoMock.Object, _applicationUserRepoMock.Object, _userWishListRepoMock.Object, _loggerMock.Object);
        }

        // ----------------------- MarkNotificationsAsRead ----------------------------------------------------------------------

        [Fact]
        public async Task MarkNotificationsAsRead_WhenUnreadNotificationsExist_ShouldMarkThemAsRead()
        {
            // Arrange
            var notifications = new List<UserNotification>
            {
                new UserNotification { UserId = "1", IsRead = false, NotificationId = 1 },
                new UserNotification { UserId = "1", IsRead = false, NotificationId = 2 }
            };

            _userNotficationRepoMock
                .Setup(r => r.GetAllByIdAsync("1", "UserId", It.IsAny<QueryOptions<UserNotification>>()))
                .ReturnsAsync(notifications);

            _userNotficationRepoMock
                .Setup(r => r.UpdateAsync(It.IsAny<UserNotification>()))
                .Returns(Task.CompletedTask);

            // Act
            await _notificationService.MarkNotificationsAsRead("1");

            // Assert
            foreach (var notification in notifications)
            {
                Assert.True(notification.IsRead);
            }

            _userNotficationRepoMock.Verify(r => r.UpdateAsync(It.IsAny<UserNotification>()), Times.Exactly(2));
        }

        [Fact]
        public async Task MarkNotificationsAsRead_WhenNoUnreadNotificationsExist_ShouldNotCallUpdate()
        {
            // Arrange
            var notifications = new List<UserNotification>(); // No unread notifications

            _userNotficationRepoMock
                .Setup(r => r.GetAllByIdAsync("1", "UserId", It.IsAny<QueryOptions<UserNotification>>()))
                .ReturnsAsync(notifications);

            // Act
            await _notificationService.MarkNotificationsAsRead("1");

            // Assert
            _userNotficationRepoMock.Verify(r => r.UpdateAsync(It.IsAny<UserNotification>()), Times.Never);
        }

        // ---------------------------------- CreateProductPriceDropNotification --------------------------------------------------------

        [Fact]
        public async Task CreateProductPriceDropNotification_ShouldCreateNonGlobalNotificationWithCorrectMessage()
        {
            // Arrange
            int productId = 1; 
            string productName = "Product";
            decimal newPrice = 1.00m,  oldPrice = 1.50m;

            Notification capturedNotif = null;
            _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Callback<Notification>(n => capturedNotif = n); // Capture the notification added to the database

            // Act
            await _notificationService.CreateProductPriceDropNotifications(productId, productName, newPrice, oldPrice);

            // Assert
            Assert.NotNull(capturedNotif);
            Assert.Equal(NotificationMessages.ProductPriceDrop(productName, newPrice, oldPrice), capturedNotif.Message);
            Assert.False(capturedNotif.IsGlobal);

        }

        [Fact]
        public async Task CreateProductPriceDropNotification_WhenExistingUsersWithItemOnWishlist_ShouldCreateUserNotifications()
        {
            // Arrange
            int productId = 1; 
            string productName = "Product";
            decimal newPrice = 1.00m, oldPrice = 1.50m;

            var wishlistItems = new UserWishList[]
            {
                new UserWishList { UserId = "User", ProductId = productId, LastCheapestPrice = oldPrice },
                new UserWishList { UserId = "User2", ProductId = productId, LastCheapestPrice = oldPrice }
            };

            Notification capturedNotif = new Notification { Id = 123 };
            _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Callback<Notification>(n =>
                {
                    n.Id = capturedNotif.Id;  // Ensure ID consistency
                    capturedNotif = n;
                });

            _userWishListRepoMock.Setup(r => r.GetAllByIdAsync(productId, "ProductId", It.IsAny<QueryOptions<UserWishList>>()))
                .ReturnsAsync(wishlistItems);

            var userNotifications = new List<UserNotification>();
            _userNotficationRepoMock.Setup(r => r.AddAsync(It.IsAny<UserNotification>()))
                .Callback<UserNotification>(un => userNotifications.Add(un));

            // Act
            await _notificationService.CreateProductPriceDropNotifications(productId, productName, newPrice, oldPrice);

            // Assert
            Assert.Equal(wishlistItems.Length, userNotifications.Count); // All wishlist users should get the notification
            Assert.NotNull(capturedNotif); 

            // Ensure the same notification ID is linked to all user notifications
            foreach (var userNotif in userNotifications)
            {
                Assert.Equal(capturedNotif.Id, userNotif.NotificationId);
                Assert.False(userNotif.IsRead);
            }

            // Verify the correct number of user notifications were added
            _userNotficationRepoMock.Verify(r => r.AddAsync(It.IsAny<UserNotification>()), Times.Exactly(wishlistItems.Length));
        }


        // ---------------------------------- CreateGlobalNotification ------------------------------------------------------------------
        [Fact]
        public async Task CreateGlobalNotification_ShouldCreateGlobalNotificationWithCorrectMessage(){
            // Arrange
            string inputMessage = "Test";

            Notification capturedNotif = null;
            _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Callback<Notification>(n => capturedNotif = n); // Capture the notification added to the database

            // Act
            await _notificationService.CreateGlobalNotification(inputMessage);

            // Assert
            Assert.NotNull(capturedNotif);
            Assert.Equal(NotificationMessages.GlobalAnnouncement(inputMessage), capturedNotif.Message);
        }

        [Fact]
        public async Task CreateGlobalNotification_WhenUsersExist_ShouldCreateUserNotificationForEveryUser(){
            // Arrange
            string inputMessage = "Test";

            var users = new ApplicationUser[]{
                new ApplicationUser{Id = "1"},
                new ApplicationUser{Id = "2"},
                new ApplicationUser{Id = "3"},
                new ApplicationUser{Id = "4"},
                new ApplicationUser{Id = "5"},
                new ApplicationUser{Id = "6"},
                new ApplicationUser{Id = "7"},
            };

            Notification capturedNotif = new Notification { Id = 123 };
            _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Callback<Notification>(n =>
                {
                    n.Id = capturedNotif.Id;  // Ensure ID consistency
                    capturedNotif = n;
                });

            _applicationUserRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var userNotifications = new List<UserNotification>();
            _userNotficationRepoMock.Setup(r => r.AddAsync(It.IsAny<UserNotification>()))
                .Callback<UserNotification>(un => userNotifications.Add(un));

            // Act
            await _notificationService.CreateGlobalNotification(inputMessage);

            // Assert
            Assert.Equal(users.Length, userNotifications.Count); // All users should get the notification
            Assert.NotNull(capturedNotif); 

            foreach (var userNotif in userNotifications)
            {
                Assert.Equal(capturedNotif.Id, userNotif.NotificationId);
                Assert.False(userNotif.IsRead);
            }

            // Verify the correct number of user notifications were added
            _userNotficationRepoMock.Verify(r => r.AddAsync(It.IsAny<UserNotification>()), Times.Exactly(users.Length));
        }
    }
}