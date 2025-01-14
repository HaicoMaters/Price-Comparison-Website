using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Price_Comparison_Website.Models
{
	public class Product
	{
		public Product()
		{
			PriceListings = new List<PriceListing>();
		}

		public int ProductId { get; set; } // PK
		public string? Name { get; set; }
		public string? Description { get; set; }
		public string? ImageUrl { get; set; }

		public int CategoryId { get; set; } // Foreign Key
		// Navigation Property
		[ValidateNever]
		public Category? Category { get; set; }
		[ValidateNever]
		// Navigation Property for PriceListings
		public ICollection<PriceListing>? PriceListings { get; set; }  // One Product can have many PriceListings
	}
}