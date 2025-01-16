using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Price_Comparison_Website.Models
{
	public class PriceListing
	{
		public int PriceListingId { get; set; } // PK
		public int ProductId { get; set; } // FK
		[ValidateNever]
		public Product Product { get; set; }
		public int VendorId { get; set; } // FK
        [ValidateNever]
        public Vendor Vendor { get; set; }
		public decimal Price { get; set; }
		public string? PurchaseUrl { get; set; }
		public DateTime? DateListed { get; set; } // When the listing was last updated
	}
}
