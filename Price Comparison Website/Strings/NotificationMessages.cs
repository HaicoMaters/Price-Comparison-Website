using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Price_Comparison_Website.Models
{
    public static class NotificationMessages
    {
        public static string ProductPriceDrop(string productName, decimal newPrice, decimal oldPrice) =>
            $"Good news! The lowest price of '{productName}' has dropped from {oldPrice.ToString("C")} to {newPrice.ToString("C")}. Check it now!";
        
        public static string GlobalAnnouncement(string message) => // Done by admins from admin panel
            $"Announcement: {message}";
    }
}