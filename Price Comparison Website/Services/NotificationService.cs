using Microsoft.Extensions.Logging;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Services
{
    public class NotificationService : INotificationService
    {
        private Repository<UserNotification> userNotifications;
        private Repository<Notification> notifications;
        private Repository<ApplicationUser> users;
        private Repository<UserWishList> wishLists;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ApplicationDbContext context, ILogger<NotificationService> logger)
        {
            userNotifications = new Repository<UserNotification>(context);
            notifications = new Repository<Notification>(context);
            users = new Repository<ApplicationUser>(context);
            wishLists = new Repository<UserWishList>(context);
            _logger = logger;
        }

        public async Task<IEnumerable<Notification>> GetAllUserNotifications(string userId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all notifications for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<Notification>> GetUnreadUserNotifications(string userId)
        {
            try
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
                        _logger.LogDebug("Notification: {Message}, IsRead: {IsRead}", notification.Message, userNotif.IsRead);
                    }
                }

                return notificationList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get unread notifications for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<Notification>> GetReadUserNotifications(string userId)
        {
            try
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
                        _logger.LogDebug("Notification: {Message}, IsRead: {IsRead}", notification.Message, userNotif.IsRead);
                    }
                }

                return notificationList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get read notifications for user {UserId}", userId);
                throw;
            }
        }

        public async Task MarkNotificationsAsRead(string userId)
        {
            try
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
                            _logger.LogError(ex, "Error marking notification {NotificationId} as read for user {UserId}", 
                                userNotif.NotificationId, userId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark notifications as read for user {UserId}", userId);
                throw;
            }
        }

        public async Task DeleteUserNotification(int notificationId, string userId)
        {
            try
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
                        _logger.LogInformation("Successfully deleted notification {NotificationId} for user {UserId}", 
                            notificationId, userId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deleting notification {NotificationId} for user {UserId}", 
                            notificationId, userId);
                        throw;
                    }
                }
                else
                {
                    _logger.LogWarning("No notification found with ID {NotificationId} for user {UserId}", 
                        notificationId, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete notification {NotificationId} for user {UserId}", 
                    notificationId, userId);
                throw;
            }
        }

        public async Task CreateProductPriceDropNotifications(int productId, string productName, decimal newPrice, decimal oldPrice)
        {
            try
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

                _logger.LogInformation("Created price drop notification for product {ProductName}. Old price: {OldPrice}, New price: {NewPrice}",
                    productName, oldPrice, newPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create price drop notification for product {ProductId}", productId);
                throw;
            }
        }

        public async Task CreateGlobalNotification(string message)
        {
            try
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

                _logger.LogInformation("Created global notification: {Message}", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create global notification with message: {Message}", message);
                throw;
            }
        }

    }
}