using Microsoft.AspNetCore.Identity;

namespace Price_Comparison_Website.Models
{
	public class ApplicationUser : IdentityUser
	{
		public ICollection<UserWishList>? WishList { get; set; }
		public ICollection<UserViewingHistory>? ViewingHistory { get; set; } // The last viewed items
		public ICollection<UserNotification> UserNotifications { get; set; }
	}
}
