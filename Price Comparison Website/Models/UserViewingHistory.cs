using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Price_Comparison_Website.Models
{
	// Join Table
	public class UserViewingHistory
	{
		public string? UserId { get; set; } // FK
		public int ProductId { get; set; } // FK
		public DateTime LastViewed { get; set; } // For purposes of ordering the view history

        // Navigation Properties
        [ValidateNever]
		public ApplicationUser User { get; set; }
		[ValidateNever]
		public Product Product { get; set; }
	}
}