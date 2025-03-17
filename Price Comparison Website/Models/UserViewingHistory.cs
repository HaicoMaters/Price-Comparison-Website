using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Price_Comparison_Website.Models
{
    // Join Table
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