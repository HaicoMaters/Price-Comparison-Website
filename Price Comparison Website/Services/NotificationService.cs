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
                    try
                    {
                        userNotif.IsRead = true;
                        await userNotifications.UpdateAsync(userNotif);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error marking notification as read: " + ex.Message);
                    }
                }
            }
        }

        public async Task DeleteUserNotification(int notificationId, string userId)
        {
            // Get all notifications for the user with the specified ID
            var userNotifs = await userNotifications.GetAllByIdAsync(userId, "UserId", new QueryOptions<UserNotification>
            {
                Where = n => n.NotificationId == notificationId
            });

            var notificationToDelete = userNotifs.FirstOrDefault();
            if (notificationToDelete != null)
            {
                try
                {
                    await userNotifications.DeleteAsync(notificationToDelete);
                    Console.WriteLine($"Successfully deleted notification {notificationId} for user {userId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting notification {notificationId}: {ex.Message}");
                    throw;
                }
            }
            else
            {
                Console.WriteLine($"No notification found with ID {notificationId} for user {userId}");
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

        public async Task CreateGlobalNotification(string message)
        {
            var notification = new Notification
            {
                Message = NotificationMessages.GlobalAnnouncement(message),
                IsGlobal = true
            };

            await notifications.AddAsync(notification);

            // Get all users
            var allUsers = await users.GetAllAsync();

            foreach (var user in allUsers)
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
        
    }
}