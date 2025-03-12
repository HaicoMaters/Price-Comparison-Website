using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Price_Comparison_Website.Models
{
	// Join Table
	public class UserWishList
	{
		public string? UserId { get; set; } // FK
		public int ProductId { get; set; } // FK
										  
		// Navigation Properties
		[ValidateNever]
		public ApplicationUser User { get; set; }
		[ValidateNever]
		public Product Product { get; set; }
		public decimal LastCheapestPrice { get; set; } // For Notifiation System
	}
}