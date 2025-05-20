using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriceComparisonWebsite.Models
{
    // <summary>
    // A join table representing the many-to-many relationship between users and products as items in their viewing history
    // </summary>
    public class UserViewingHistory
    {
        [Key]
        [Column(Order = 0)]
        [Required]
        public string UserId { get; set; } // FK

        [Key]
        [Column(Order = 1)]
        [Required]
        public int ProductId { get; set; } // FK

        [Required]
        public DateTime LastViewed { get; set; } // For purposes of ordering the view history

        // Navigation Properties
        [ValidateNever]
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ValidateNever]
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}