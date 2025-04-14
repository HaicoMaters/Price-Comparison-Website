using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceComparisonWebsite.Models
{
    // Join table for User and Notification
    public class UserNotification
    {
        [Key]
        [Column(Order = 0)]
        [Required]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        public int NotificationId { get; set; }

        [ValidateNever]
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ValidateNever]
        [ForeignKey("NotificationId")]
        public Notification Notification { get; set; }

        [Required]
        public bool IsRead { get; set; } = false; // Default to unread
    }
}