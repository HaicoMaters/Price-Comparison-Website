using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PriceComparisonWebsite.Models
{
    //  <summary>
    // Represents a notification that can be sent to users
    // </summary>
    public class Notification
    {
        [Key]
        public int Id { get; set; } // PK

        [Required]
        [MaxLength(500)]
        public string Message { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public bool IsGlobal { get; set; }

        [ValidateNever]
        public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    }
}