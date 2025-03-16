using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Price_Comparison_Website.Models
{
	public class PriceListing
	{
		public int PriceListingId { get; set; } // PK
		
		[Required]
		public int ProductId { get; set; } // FK
		[ValidateNever]
		public Product? Product { get; set; }
		
		[Required(ErrorMessage = "Please select a vendor")]
		[Range(1, int.MaxValue, ErrorMessage = "Please select a valid vendor")]
		public int VendorId { get; set; } // FK       
	 	[ValidateNever]      
        public Vendor? Vendor { get; set; }		
		
		[Required]
		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
		[DataType(DataType.Currency)]
		public decimal Price { get; set; }
		
		public decimal DiscountedPrice { get; set; } // For discounts, default to Price if no discount
		
		[Required]
		[Url(ErrorMessage = "Please enter a valid URL")]
		[Display(Name = "Purchase URL")]
		public string PurchaseUrl { get; set; }
		
		public DateTime DateListed { get; set; } // When the listing was last updated
	}

    public class LessThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public LessThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }
    }
}
