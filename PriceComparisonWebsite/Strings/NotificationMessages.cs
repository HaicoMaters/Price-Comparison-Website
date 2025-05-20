using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Models
{
    /// <summary>
    /// Provides standardized message templates for system notifications
    /// </summary>
    public static class NotificationMessages
    {
        /// <summary>
        /// Creates a notification message for when a product's price drops
        /// </summary>
        /// <param name="productName">The name of the product</param>
        /// <param name="newPrice">The new lower price</param>
        /// <param name="oldPrice">The previous price</param>
        /// <returns>A formatted notification message</returns>
        public static string ProductPriceDrop(string productName, decimal newPrice, decimal oldPrice) =>
            $"Good news! The lowest price of '{productName}' has dropped from {oldPrice.ToString("C")} to {newPrice.ToString("C")}. Check it now!";
        
        /// <summary>
        /// Creates a notification message for global announcements
        /// </summary>
        /// <param name="message">The announcement message</param>
        /// <returns>A formatted announcement message</returns>
        public static string GlobalAnnouncement(string message) => // Done by admins from admin panel
            $"Announcement: {message}";
    }
}