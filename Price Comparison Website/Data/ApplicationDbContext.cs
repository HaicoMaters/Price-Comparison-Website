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
		public DbSet<UserWishList> UserWishLists { get; set; }
		public DbSet<UserViewingHistory> UserViewingHistories { get; set; }

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

			// Many-to-many relationship between ApplicationUser and Product for WishList
			builder.Entity<UserWishList>()
				.HasKey(uw => new { uw.UserId, uw.ProductId });

			builder.Entity<UserWishList>()
				.HasOne(uw => uw.User)
				.WithMany(u => u.WishList)
				.HasForeignKey(uw => uw.UserId);

			builder.Entity<UserWishList>()
				.HasOne(uw => uw.Product)
				.WithMany()
				.HasForeignKey(uw => uw.ProductId);

			// Many-to-many relationship between ApplicationUser and Product for ViewingHistory
			builder.Entity<UserViewingHistory>()
				.HasKey(uv => new { uv.UserId, uv.ProductId });

			builder.Entity<UserViewingHistory>()
				.HasOne(uv => uv.User)
				.WithMany(u => u.ViewingHistory)
				.HasForeignKey(uv => uv.UserId);

			builder.Entity<UserViewingHistory>()
				.HasOne(uv => uv.Product)
				.WithMany()
				.HasForeignKey(uv => uv.ProductId);

		}
	}
}
