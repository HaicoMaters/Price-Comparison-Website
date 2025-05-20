using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PriceComparisonWebsite.Models
{
    /// <summary>
    /// Custom application user class extending IdentityUser with additional properties
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [ValidateNever]
        public ICollection<UserWishList> WishList { get; set; } = new List<UserWishList>();

        [ValidateNever]
        public ICollection<UserViewingHistory> ViewingHistory { get; set; } = new List<UserViewingHistory>();

        [ValidateNever]
        public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    }
}
