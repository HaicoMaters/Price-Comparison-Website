using Microsoft.Extensions.Logging;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Services
{
    public class NotificationService
    {
        private Repository<UserNotification> userNotifications;
        private Repository<Notification> notifications;
        private Repository<ApplicationUser> users;
        private Repository<UserWishList> wishLists;

        public NotificationService(ApplicationDbContext context)
        {
            userNotifications = new Repository<UserNotification>(context);
            notifications = new Repository<Notification>(context);
            users = new Repository<ApplicationUser>(context);
            wishLists = new Repository<UserWishList>(context);
        }

        public async Task<IEnumerable<Notification>> GetAllUserNotifications(string userId)
        {
            // Get all notifications for a user (both read and unread)
            var userNotifs = await userNotifications.GetAllByIdAsync(userId, "UserId", new QueryOptions<UserNotification>
            {
                Includes = "Notification",
                OrderBy = n => n.Notification.CreatedAt
            });

            var notificationList = new List<Notification>();

            foreach (var userNotif in userNotifs) // Not a great solution but it works `¯\_(ツ)_/¯`
            {
                var notification = await notifications.GetByIdAsync(userNotif.NotificationId, new QueryOptions<Notification>());
                if (notification != null)
                {
                    notificationList.Add(notification);
                }
            }

            return notificationList;
        }


        public async Task<IEnumerable<Notification>> GetUnreadUserNotifications(string userId)
        {
            // Get all unread notifications for a user
            var userNotifs = await userNotifications.GetAllByIdAsync(userId, "UserId", new QueryOptions<UserNotification>
            {
                Includes = "Notification",
                Where = n => !n.IsRead,
                OrderBy = n => n.Notification.CreatedAt
            });

             var notificationList = new List<Notification>();

            foreach (var userNotif in userNotifs) // Not a great solution but it works `¯\_(ツ)_/¯`
            {
                var notification = await notifications.GetByIdAsync(userNotif.NotificationId, new QueryOptions<Notification>());
                if (notification != null)
                {
                    notificationList.Add(notification);
                    Console.WriteLine("Notification: " + notification.Message + " IsRead: " + userNotif.IsRead);
                }
            }

            return notificationList;
        }

        public async Task<IEnumerable<Notification>> GetReadUserNotifications(string userId)
        {
            // Get all read notifications for a user
            var userNotifs = await userNotifications.GetAllByIdAsync(userId, "UserId", new QueryOptions<UserNotification>
            {
                Includes = "Notification",
                Where = n => n.IsRead,
                OrderBy = n => n.Notification.CreatedAt
            });

             var notificationList = new List<Notification>();

            foreach (var userNotif in userNotifs) // Not a great solution but it works `¯\_(ツ)_/¯`
            {
                var notification = await notifications.GetByIdAsync(userNotif.NotificationId, new QueryOptions<Notification>());
                if (notification != null)
                {
                    notificationList.Add(notification);
                    Console.WriteLine("Notification: " + notification.Message + " IsRead: " + userNotif.IsRead);
                }
            }

            return notificationList;
        }

        public async Task MarkNotificationsAsRead(string userId)
        {
            // Get all unread notifications for a user
            var userNotifs = await userNotifications.GetAllByIdAsync(userId, "UserId", new QueryOptions<UserNotification>
            {
                Where = n => !n.IsRead
            });

            // Mark notifications as read
            foreach (var userNotif in userNotifs)
            {
                if (!userNotif.IsRead)
                {
                    userNotif.IsRead = true;
                    await userNotifications.UpdateAsync(userNotif);
                }
            }
        }

        public async Task CreateGlobalNotification(string message)
        {
            // Create the global notification
            var notification = new Notification
            {
                Message = NotificationMessages.GlobalAnnouncement(message),
                IsGlobal = true // Mark as a global notification
            };

            await notifications.AddAsync(notification);

            // Associate notification with all users
            var sendees = await users.GetAllAsync();

            foreach (var user in sendees)
            {
                var userNotification = new UserNotification
                {
                    UserId = user.Id,
                    NotificationId = notification.Id,
                    IsRead = false
                };

                await userNotifications.AddAsync(userNotification);
            }
        }

        public async Task CreateProductPriceDropNotifications(int productId, string productName, decimal newPrice, decimal oldPrice)
        {
            // Create the product price drop notification
            var notification = new Notification
            {
                Message = NotificationMessages.ProductPriceDrop(productName, newPrice, oldPrice),
                IsGlobal = false // Mark as a product price drop notification
            };

            await notifications.AddAsync(notification);

            var wishlistItems = await wishLists.GetAllByIdAsync(productId, "ProductId", new QueryOptions<UserWishList>());

            foreach (var wishlistItem in wishlistItems)
            {
                var userNotification = new UserNotification
                {
                    UserId = wishlistItem.UserId,
                    NotificationId = notification.Id,
                    IsRead = false
                };

                await userNotifications.AddAsync(userNotification);
            }
        }
    }
}