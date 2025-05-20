using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceComparisonWebsite.Models
{
    /// <summary>
	/// A join table representing the many-to-many relationship between users and notifications
	/// </summary>
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