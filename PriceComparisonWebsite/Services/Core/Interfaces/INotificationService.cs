using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetAllUserNotifications(string userId);
        Task<IEnumerable<Notification>> GetUnreadUserNotifications(string userId);
        Task<IEnumerable<Notification>> GetReadUserNotifications(string userId);
        Task MarkNotificationsAsRead(string userId);
        Task DeleteUserNotification(int notificationId, string userId);
        Task CreateProductPriceDropNotifications(int productId, string productName, decimal newPrice, decimal oldPrice);
        Task CreateGlobalNotification(string message);
    }
}
