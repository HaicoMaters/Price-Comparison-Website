using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace PriceComparisonWebsite.Models
{
	public class Product
	{
		public Product()
		{
			Name = string.Empty;
			Description = string.Empty;
			ImageUrl = string.Empty;
			PriceListings = new List<PriceListing>();
		}

		public int ProductId { get; set; } // PK
		
		[Required(ErrorMessage = "Product name is required")]
		[StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
		public string Name { get; set; }
		
		[Required(ErrorMessage = "Description is required")]
		[MinLength(5, ErrorMessage = "Description must be at least 5 characters long")]
		public string Description { get; set; }
		
		[Required(ErrorMessage = "Image URL is required")]
		[Url(ErrorMessage = "Please enter a valid URL")]
		[Display(Name = "Image URL")]
		public string ImageUrl { get; set; }
		
		[Required(ErrorMessage = "Please select a category")]
		[Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
		public int CategoryId { get; set; } // Foreign Key
		
		public decimal CheapestPrice { get; set; }
		
		// Navigation Property
		[ValidateNever]
		public Category? Category { get; set; }
		
		[ValidateNever]
		// Navigation Property for PriceListings
		public ICollection<PriceListing>? PriceListings { get; set; }  // One Product can have many PriceListings
	}
}