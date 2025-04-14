using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceComparisonWebsite.Models
{
    public class UserWishList
    {
        [Key]
        [Column(Order = 0)]
        [Required]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [Required]
        public int ProductId { get; set; }

        [ValidateNever]
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ValidateNever]
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal LastCheapestPrice { get; set; }
    }
}