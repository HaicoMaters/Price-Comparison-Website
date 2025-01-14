using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		// Create Tables
		public DbSet<Product> Products { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Vendor> Vendors { get; set; }
		public DbSet<PriceListing> PriceListings { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Each Category Has many Products
			builder.Entity<Product>()
				.HasOne(p => p.Category)
				.WithMany(c => c.Products)
				.HasForeignKey(p => p.CategoryId);

			// Many Listings for one Product
			builder.Entity<PriceListing>()
				.HasOne(pl => pl.Product)
				.WithMany(p => p.PriceListings)  // One Product can have many PriceListings
				.HasForeignKey(pl => pl.ProductId);

			// Vendors can have many Listings
			builder.Entity<PriceListing>()
				.HasOne(pl => pl.Vendor)
				.WithMany(v => v.PriceListings)  // One Vendor can have many PriceListings
				.HasForeignKey(pl => pl.VendorId);

			// Many-to-many relationship between ApplicationUser and Product (Wishlist)
			builder.Entity<ApplicationUser>()
				.HasMany(u => u.WishList)
				.WithMany()
				.UsingEntity(j => j.ToTable("UserWishList"));  // Creates the UserWishList join table

			// Many-to-many relationship between ApplicationUser and Product (ViewingHistory)
			builder.Entity<ApplicationUser>()
				.HasMany(u => u.ViewingHistory)
				.WithMany()
				.UsingEntity(j => j.ToTable("UserViewingHistory"));  // Creates the UserViewingHistory join table
		}
	}
}
