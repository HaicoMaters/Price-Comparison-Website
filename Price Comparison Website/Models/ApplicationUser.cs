using Microsoft.AspNetCore.Identity;

namespace Price_Comparison_Website.Models
{
	public class ApplicationUser : IdentityUser
	{
		// IF HAVING ISSUES LATER IN DEVELOPMENT REMEBER TO SET THESE TO A FRESH LIST WHEN CREATING AN ACCOUNT
		public ICollection<UserWishList>? WishList { get; set; }
		public ICollection<UserViewingHistory>? ViewingHistory { get; set; } // The last viewed items
	}
}
