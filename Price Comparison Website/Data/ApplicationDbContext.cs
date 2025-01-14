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


			// Seed Data

			//Categories
			builder.Entity<Category>().HasData(
				new Category { CategoryId = 1, Name = "Electronics" },
				new Category { CategoryId = 2, Name = "Tools" },
				new Category { CategoryId = 3, Name = "Clothing" },
				new Category { CategoryId = 4, Name = "Food" },
				new Category { CategoryId = 5, Name = "Pet Items" },
				new Category { CategoryId = 6, Name = "Toys" },
				new Category { CategoryId = 7, Name = "Stationary" },
				new Category { CategoryId = 8, Name = "Drinks" },
				new Category { CategoryId = 9, Name = "Games" },
				new Category { CategoryId = 10, Name = "Books" },
				new Category { CategoryId = 11, Name = "Furniture" },
				new Category { CategoryId = 12, Name = "Beauty" },
				new Category { CategoryId = 13, Name = "Sports" },
				new Category { CategoryId = 14, Name = "Health" },
				new Category { CategoryId = 15, Name = "Music" },
				new Category { CategoryId = 16, Name = "Movies" },
				new Category { CategoryId = 17, Name = "Travel" },
				new Category { CategoryId = 18, Name = "Gifts" }
				);
			
			// Products
			builder.Entity<Product>().HasData(
				new Product { ProductId = 1, Name = "Laptop", Description = "A high-performance laptop", ImageUrl = "laptop.jpg", CategoryId = 1 },
				new Product { ProductId = 2, Name = "Smartphone", Description = "Latest smartphone model", ImageUrl = "smartphone.jpg", CategoryId = 1 },
				new Product { ProductId = 3, Name = "Tablet", Description = "Portable tablet for all your needs", ImageUrl = "tablet.jpg", CategoryId = 1 },
				new Product { ProductId = 4, Name = "Hammer", Description = "A durable hammer for all your needs", ImageUrl = "hammer.jpg", CategoryId = 2 },
				new Product { ProductId = 5, Name = "Screwdriver Set", Description = "Comprehensive screwdriver set", ImageUrl = "screwdriverset.jpg", CategoryId = 2 },
				new Product { ProductId = 6, Name = "T-shirt", Description = "Cotton T-shirt in various colors", ImageUrl = "tshirt.jpg", CategoryId = 3 },
				new Product { ProductId = 7, Name = "Jeans", Description = "Stylish denim jeans", ImageUrl = "jeans.jpg", CategoryId = 3 },
				new Product { ProductId = 8, Name = "Jacket", Description = "Warm winter jacket", ImageUrl = "jacket.jpg", CategoryId = 3 },
				new Product { ProductId = 9, Name = "Canned Soup", Description = "Ready-to-eat canned soup", ImageUrl = "cannedsoup.jpg", CategoryId = 4 },
				new Product { ProductId = 10, Name = "Dog Toy", Description = "A squeaky dog toy", ImageUrl = "dogtoy.jpg", CategoryId = 5 },
				new Product { ProductId = 11, Name = "Cat Food", Description = "Healthy food for your cat", ImageUrl = "catfood.jpg", CategoryId = 5 },
				new Product { ProductId = 12, Name = "Building Blocks", Description = "Colorful building blocks for kids", ImageUrl = "blocks.jpg", CategoryId = 6 },
				new Product { ProductId = 13, Name = "Action Figure", Description = "A detailed action figure", ImageUrl = "actionfigure.jpg", CategoryId = 6 },
				new Product { ProductId = 14, Name = "Notebook", Description = "A spiral-bound notebook", ImageUrl = "notebook.jpg", CategoryId = 7 },
				new Product { ProductId = 15, Name = "Pen Set", Description = "High-quality pens in a set", ImageUrl = "penset.jpg", CategoryId = 7 },
				new Product { ProductId = 16, Name = "Orange Juice", Description = "Freshly squeezed orange juice", ImageUrl = "orangejuice.jpg", CategoryId = 8 },
				new Product { ProductId = 17, Name = "Sparkling Water", Description = "Refreshing sparkling water", ImageUrl = "sparklingwater.jpg", CategoryId = 8 },
				new Product { ProductId = 18, Name = "Chocolate Bar", Description = "Delicious milk chocolate bar", ImageUrl = "chocolatebar.jpg", CategoryId = 8 },
				new Product { ProductId = 19, Name = "Chess Set", Description = "Wooden chess set", ImageUrl = "chessset.jpg", CategoryId = 9 },
				new Product { ProductId = 20, Name = "Novel", Description = "A thrilling new novel", ImageUrl = "novel.jpg", CategoryId = 10 },
				new Product { ProductId = 21, Name = "Science Book", Description = "An insightful science book", ImageUrl = "sciencebook.jpg", CategoryId = 10 },
				new Product { ProductId = 22, Name = "Cookbook", Description = "A cookbook full of delicious recipes", ImageUrl = "cookbook.jpg", CategoryId = 10 },
				new Product { ProductId = 23, Name = "Coffee Table", Description = "A modern coffee table", ImageUrl = "coffeetable.jpg", CategoryId = 11 },
				new Product { ProductId = 24, Name = "Sofa", Description = "Comfortable 3-seater sofa", ImageUrl = "sofa.jpg", CategoryId = 11 },
				new Product { ProductId = 25, Name = "Shampoo", Description = "Shampoo for all hair types", ImageUrl = "shampoo.jpg", CategoryId = 12 },
				new Product { ProductId = 26, Name = "Football", Description = "High-quality football", ImageUrl = "football.jpg", CategoryId = 13 },
				new Product { ProductId = 27, Name = "Baseball Bat", Description = "Wooden baseball bat", ImageUrl = "baseballbat.jpg", CategoryId = 13 },
				new Product { ProductId = 28, Name = "Vitamins", Description = "Multivitamins for daily health", ImageUrl = "vitamins.jpg", CategoryId = 14 },
				new Product { ProductId = 29, Name = "Protein Powder", Description = "Whey protein powder", ImageUrl = "proteinpowder.jpg", CategoryId = 14 },
				new Product { ProductId = 30, Name = "Yoga Mat", Description = "Non-slip yoga mat", ImageUrl = "yogamat.jpg", CategoryId = 14 }
		);
			// Vendors
			builder.Entity<Vendor>().HasData(
				new Vendor { VendorId = 1, Name = "Amazon", VendorUrl = "https://www.amazon.com" },
				new Vendor { VendorId = 2, Name = "eBay", VendorUrl = "https://www.ebay.com" },
				new Vendor { VendorId = 3, Name = "Best Buy", VendorUrl = "https://www.bestbuy.com" },
				new Vendor { VendorId = 4, Name = "Target", VendorUrl = "https://www.target.com" },
				new Vendor { VendorId = 5, Name = "Home Depot", VendorUrl = "https://www.homedepot.com" },
				new Vendor { VendorId = 6, Name = "Ikea", VendorUrl = "https://www.ikea.com" },
				new Vendor { VendorId = 7, Name = "AliExpress", VendorUrl = "https://www.aliexpress.com" },
				new Vendor { VendorId = 8, Name = "Etsy", VendorUrl = "https://www.etsy.com" }
		);

			// Price Listings
		}
	}
}
