using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services
{
    /// <summary>
    /// Service for managing user notifications and announcements
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Gets all notifications for a user
        /// </summary>
        Task<IEnumerable<Notification>> GetAllUserNotifications(string userId);

        /// <summary>
        /// Gets all unread notifications for a user
        /// </summary>
        Task<IEnumerable<Notification>> GetUnreadUserNotifications(string userId);

        /// <summary>
        /// Gets all read notifications for a user
        /// </summary>
        Task<IEnumerable<Notification>> GetReadUserNotifications(string userId);

        /// <summary>
        /// Marks all notifications as read for a user
        /// </summary>
        Task MarkNotificationsAsRead(string userId);

        /// <summary>
        /// Deletes a specific notification for a user
        /// </summary>
        Task DeleteUserNotification(int notificationId, string userId);

        /// <summary>
        /// Creates notifications for users when a product's price drops
        /// </summary>
        Task CreateProductPriceDropNotifications(int productId, string productName, decimal newPrice, decimal oldPrice);

        /// <summary>
        /// Creates a global notification that will be sent to all users
        /// </summary>
        Task CreateGlobalNotification(string message);
    }
}
