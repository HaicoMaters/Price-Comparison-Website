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
		public DbSet<Notification> Notifications { get; set; }
		public DbSet<UserNotification> UserNotifications { get; set; }

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
				.HasForeignKey(pl => pl.ProductId)
				.OnDelete(DeleteBehavior.Cascade);

			// Vendors can have many Listings
			builder.Entity<PriceListing>()
				.HasOne(pl => pl.Vendor)
				.WithMany(v => v.PriceListings)  // One Vendor can have many PriceListings
				.HasForeignKey(pl => pl.VendorId)
				.OnDelete(DeleteBehavior.Cascade);

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
			
			builder.Entity<UserNotification>().HasKey(un => new { un.UserId, un.NotificationId }); // Composite Key

			builder.Entity<UserNotification>()
				.HasOne(un => un.User)
				.WithMany(u => u.UserNotifications)
				.HasForeignKey(un => un.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<UserNotification>()
				.HasOne(un => un.Notification)
				.WithMany(n => n.UserNotifications)
				.HasForeignKey(un => un.NotificationId)
				.OnDelete(DeleteBehavior.Cascade);


			// Seed Data (Used AI Assistance just to have large quantity

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
				new Product { ProductId = 1, Name = "Turtle Beach Stealth 600 Gen 3 Wireless Multiplatform Amplified Gaming Headset", Description = "Multiplatform, Low-latency 2.4GHz Wireless + Bluetooth 5.2\r\nBest-in-Class 80-Hour Battery Life with Quick Charge\r\nQuickSwitch Button for Seamless Wireless to Bluetooth switching\r\nFlip-to-Mute Mic with A.I.-Based Noise Reduction\r\nPowerful, 50mm Nanoclear Drivers for Vibrant Spatial Audio\r\nSwarm II Desktop & Mobile App with Advanced 10-Band EQ\r\nMappable Wheel & Mode Button for Customizable Functions", ImageUrl = "https://m.media-amazon.com/images/I/71kECPK7CXL._AC_SL1500_.jpg", CategoryId = 1 ,CheapestPrice = 99.00m }
		);
			// Vendors
			builder.Entity<Vendor>().HasData(
				new Vendor { VendorId = 1, Name = "Amazon", VendorUrl = "https://www.amazon.com", VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/a/a9/Amazon_logo.svg" },
				new Vendor { VendorId = 2, Name = "eBay", VendorUrl = "https://www.ebay.com", VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/1/1b/EBay_logo.svg" },
				new Vendor { VendorId = 3, Name = "Best Buy", VendorUrl = "https://www.bestbuy.com", VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/2/2f/Best_Buy_Logo.svg" },
				new Vendor { VendorId = 4, Name = "Target", VendorUrl = "https://www.target.com", VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/9/9a/Target_logo.svg" },
				new Vendor { VendorId = 5, Name = "Home Depot", VendorUrl = "https://www.homedepot.com", VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/5/5f/TheHomeDepot.svg" },
				new Vendor { VendorId = 6, Name = "Ikea", VendorUrl = "https://www.ikea.com", VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/8/8b/Ikea_logo.svg" },
				new Vendor { VendorId = 7, Name = "AliExpress", VendorUrl = "https://www.aliexpress.com", VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/f/f9/AliExpress_logo.svg" },
				new Vendor { VendorId = 8, Name = "Etsy", VendorUrl = "https://www.etsy.com", VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/8/82/Etsy_logo.svg" }
		);

			// Price Listings
			builder.Entity<PriceListing>().HasData(
				new PriceListing { PriceListingId = 1, VendorId = 1, ProductId = 1, PurchaseUrl = "https://www.amazon.com/Stealth-Wireless-Multiplatform-Amplified-Headset-Nintendo/dp/B0CYWFH5Y9/ref=sr_1_5?_encoding=UTF8&content-id=amzn1.sym.12129333-2117-4490-9c17-6d31baf0582a&dib=eyJ2IjoiMSJ9.VGoru17L34M5u4AqX0EqqypmcGNBhQxULfjaHbYVNmd0PXtUKoq0IryVhEe8Avp17c7W4F1avbJAkvdvMH3jBAvS1y-h85YgufTd1_YFIBFyMR3ugPGW3V_AdDjgteUFyhz_Eez0nfm7auWFQlzkPy2RTQwsDVHjaVrtwgAkM3xC_LGBLXpf8WBiOQfNuzqJezm6DyoWKDfnAMQK88unx_KwWs3-xqdFcuBtzcNb5QU.kSL29Wzha8iyoHFr3XL-ZPQOSlLNXOyLBsq88fLMoBg&dib_tag=se&keywords=gaming+headsets&pd_rd_r=2d291fd7-dfbe-4e94-a175-382f3c52d742&pd_rd_w=WK5FS&pd_rd_wg=SgrUc&pf_rd_p=12129333-2117-4490-9c17-6d31baf0582a&pf_rd_r=K2AVFG3GKGVRSJCX847Q&qid=1737064698&sr=8-5&th=1", Price = 99.00m, DiscountedPrice = 99.00m, DateListed = DateTime.Now },
				new PriceListing { PriceListingId = 2, VendorId = 2, ProductId = 1, PurchaseUrl = "https://www.ebay.co.uk/itm/286226735878", Price = 190.36m, DiscountedPrice = 110.21m, DateListed = DateTime.Now }
		);
		}
	}
}
