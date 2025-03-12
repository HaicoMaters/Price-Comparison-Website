using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Price_Comparison_Website.Models
{
    // Join table for User and Notification
    public class UserNotification
    {
        public string UserId { get; set; }
        [ValidateNever]
        public ApplicationUser User { get; set; }

        public int NotificationId { get; set; }
        
        [ValidateNever]
        public Notification Notification { get; set; }

        public bool IsRead { get; set; } = false; // Default to unread
    }
}