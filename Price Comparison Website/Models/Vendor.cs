using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Price_Comparison_Website.Models
{
	// The websites selling the products
	public class Vendor
	{
		public Vendor()
		{
			PriceListings = new List<PriceListing>();
		}

		public int VendorId { get; set; } // PK
		public string? Name { get; set; }
		public string? VendorUrl { get; set; }
		[ValidateNever]
		// Navigation Property for PriceListings
		public ICollection<PriceListing>? PriceListings { get; set; }  // One Vendor can have many PriceListings
	}
}
