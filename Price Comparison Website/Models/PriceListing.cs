using System.ComponentModel.DataAnnotations;

namespace Price_Comparison_Website.Models
{
	public class PriceListing
	{
		public int PriceListingId { get; set; } // PK
		public int ProductId { get; set; } // FK
		public Product Product { get; set; }
		public int VendorId { get; set; } // FK
		public Vendor Vendor { get; set; }
		public decimal Price { get; set; }
		public string? PurchaseUrl { get; set; }
		public DateTime? DateListed { get; set; }
	}
}
