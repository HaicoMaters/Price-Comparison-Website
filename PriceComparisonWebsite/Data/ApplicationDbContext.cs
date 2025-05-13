using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Data
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
		public DbSet<LoginActivity> LoginActivities { get; set; }
		public DbSet<ScraperStatus> ScraperStatus { get; set; }
		public DbSet<ProductPriceHistory> ProductPriceHistories { get; set; }

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

			// Add LoginActivity configuration
			builder.Entity<LoginActivity>()
				.HasOne(la => la.User)
				.WithMany()
				.HasForeignKey(la => la.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			// Seed Data
			builder.Entity<ScraperStatus>().HasData(
				new ScraperStatus { Id = 1, LastUpdated = DateTime.MinValue}
			);

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
				new Product { 
					ProductId = 1, 
					Name = "Pepsi Max No Sugar Cola Cans 24 x 330ml", 
					Description = "Diet type: VegetarianIngredients:Carbonated Water, Colour (E150d), Sweeteners (Aspartame, Acesulfame K), Acids (Phosphoric Acid, Citric Acid), Flavourings (Including Caffeine), Preservative (Potassium Sorbate). Contains a Source of Phenylalanin.", 
					ImageUrl = "https://m.media-amazon.com/images/I/61zIvU-0TDL.__AC_SX300_SY300_QL70_ML2_.jpg", 
					CategoryId = 4, 
					CheapestPrice = 7.50m
				},
				new Product { 
					ProductId = 2, 
					Name = "Gorilla Super Glue, 15g", 
					Description = "IMPACT TOUGH Formulated for impact resistance and strength. FAST SETTING Dries in just 10 - 45 seconds, with no gripping required. ANTI-CLOG CAP Keeps Super Glue from drying out. BONDS Metal, wood, ceramic, paper, rubber & plastics (not PP or PE) and more! FILL LINE Bottle is not full, it is filled to 15g fill line to allow liquid to flow. PREPARATION Protect work area from spills. Clean and dry the surfaces to be bonded. To puncture seal on tube, tighten white nozzle firmly. APPLY Apply a small amount of Gorilla Super Glue to one surface. Only one drop per 6.5cm2 recommended. Set time can vary based on amount of glue and type of surface glued. Excess glue can cause delayed or failed bond. PRESS Press the two surfaces together between 10-45 seconds. Wait 24 hours for full cure", 
					ImageUrl = "https://m.media-amazon.com/images/I/81GVBVWnryS._AC_SX679_.jpg", 
					CategoryId = 2, 
					CheapestPrice = 4.87m
				},
				new Product { 
					ProductId = 3, 
					Name = "UNO", 
					Description = "The classic card game of matching colors and numbers. Special Action Cards and Wild Cards for unexpected excitement and game-changing fun. Use the Swap Hands cards to change hands with any other opponent. Write your own rules for game play with the Customizable Wild cards. Players take turns matching a card in their hand with the color or number of the card shown on the top of the deck. Special graphic symbols have been added to each card to help identify the color(s) on that card. This will allow players with ANY form of color blindness to easily play! Don't forget to shout \"UNO\" when you only have one card remaining!", 
					ImageUrl = "https://m.media-amazon.com/images/I/71MrrNB7jCL._AC_SX679_.jpg", 
					CategoryId = 9, 
					CheapestPrice = 3.85m
				},
				new Product {
					ProductId = 4,
					Name = "AMD Ryzen 7 9700X Processor",
					Description = "The AMD Ryzen 7 9700X is a high-performance processor designed for gaming and content creation. It features 8 cores and 16 threads, with a base clock speed of 3.8 GHz and a boost clock speed of up to 4.5 GHz. The processor is built on the 7nm process technology and supports DDR4 memory up to 3200 MHz. It also includes support for PCIe 4.0, allowing for faster data transfer rates with compatible devices. The Ryzen 7 9700X is compatible with AM4 motherboards and has a TDP of 105W.",
					ImageUrl = "https://m.media-amazon.com/images/I/61F3ZChalyL._AC_SX679_.jpg",
					CategoryId = 1,
					CheapestPrice = 280.17m
				},
				new Product {
					ProductId = 5,
					Name = "Acer Aspire 14 AI Copilot+ PC 14.0 Touchscreen Laptop Ultra 7 256V 16GB RAM 1TB SSD Windows 11 Home A14-52M-72FH",
					Description = "Intel Core Ultra 7 256V, 16GB LPDDR5X Onboard Memory Memory / 1 TB PCIe SSD, Intel Arc Graphics 140V, Non-Touch Screen, 1920 x 1200, Windows 11 Home",
					ImageUrl = "https://c1.neweggimages.com/ProductImageCompressAll60/34-360-378-03.jpg",
					CategoryId = 1,
					CheapestPrice = 827.99m
				}
			);

			// Vendors
			builder.Entity<Vendor>().HasData(
				new Vendor { 
					VendorId = 1, 
					Name = "Amazon", 
					VendorUrl = "https://www.amazon.co.uk", 
					VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/a/a9/Amazon_logo.svg",
					SupportsAutomaticUpdates = true
				},
				new Vendor { 
					VendorId = 2, 
					Name = "eBay", 
					VendorUrl = "https://www.ebay.co.uk", 
					VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/1/1b/EBay_logo.svg"
				},
				new Vendor { 
					VendorId = 11, 
					Name = "Tesco", 
					VendorUrl = "https://www.tesco.com/", 
					VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/b/b0/Tesco_Logo.svg/2560px-Tesco_Logo.svg.png"
				},
				new Vendor {
					VendorId = 3,
					Name = "Newegg",
					VendorUrl = "https://www.newegg.com",
					VendorLogoUrl = "https://c1.neweggimages.com/WebResource/Themes/Nest/logos/Newegg_full_color_logo_RGB.SVG",
					SupportsAutomaticUpdates = true
				}
			);

			// Price Listings
			builder.Entity<PriceListing>().HasData(
				new PriceListing { 
					PriceListingId = 1, 
					ProductId = 1, 
					VendorId = 1, 
					Price = 15.60m, 
					PurchaseUrl = "https://www.amazon.co.uk/Pepsi-Max-Cans-330ml-Pack/dp/B017NVHSF8/259-3693489-2210466",
					DateListed = DateTime.Parse("2025-03-14 14:52:37.4133597"),
					DiscountedPrice = 7.50m
				},
				new PriceListing { 
					PriceListingId = 2, 
					ProductId = 2, 
					VendorId = 1, 
					Price = 10.79m, 
					PurchaseUrl = "https://www.amazon.co.uk/Gorilla-4044205-Superglue-15g/dp/B003CT4XT0/259-3693489-2210466",
					DateListed = DateTime.Parse("2025-03-14 14:52:45.2833811"),
					DiscountedPrice = 4.87m
				},
				new PriceListing { 
					PriceListingId = 3, 
					ProductId = 1, 
					VendorId = 11, 
					Price = 11.50m, 
					PurchaseUrl = "https://www.tesco.com/groceries/en-GB/products/282774907",
					DateListed = DateTime.Parse("2025-03-14 14:52:39.7688507"),
					DiscountedPrice = 11.50m
				},
				new PriceListing { 
					PriceListingId = 4, 
					ProductId = 3, 
					VendorId = 1, 
					Price = 12.85m, 
					PurchaseUrl = "https://www.amazon.co.uk/UNO-W2087-Card-Game-European/dp/B005I5M2F8",
					DateListed = DateTime.Parse("2025-03-14 14:52:51.8122214"),
					DiscountedPrice = 5.03m
				},
				new PriceListing { 
					PriceListingId = 5, 
					ProductId = 3, 
					VendorId = 2, 
					Price = 3.85m, 
					PurchaseUrl = "https://www.ebay.co.uk/itm/175594341144",
					DateListed = DateTime.Parse("2025-03-14 14:52:49.6649873"),
					DiscountedPrice = 3.85m
				},
				new PriceListing { 
					PriceListingId = 16, 
					ProductId = 4, 
					VendorId = 3, 
					Price = 327.59m, 
					PurchaseUrl = "https://www.newegg.com/global/uk-en/amd-ryzen-7-9700x-ryzen-7-9000-series-granite-ridge-socket-am5-processor/p/N82E16819113843",
					DateListed = DateTime.Parse("2025-03-14 14:52:55.8122214"),
					DiscountedPrice = 319.19m
				},
				new PriceListing { 
					PriceListingId = 17, 
					ProductId = 4, 
					VendorId = 1, 
					Price = 298.00m, 
					PurchaseUrl = "https://www.amazon.co.uk/AMD-Ryzensets-9700X-Processor-frequency/dp/B0D6NMDNNX",
					DateListed = DateTime.Parse("2025-03-14 14:52:57.8122214"),
					DiscountedPrice = 280.17m
				},
				new PriceListing { 
					PriceListingId = 18, 
					ProductId = 5, 
					VendorId = 3, 
					Price = 827.99m, 
					PurchaseUrl = "https://www.newegg.com/global/uk-en/aspire-14-ai/p/N82E16834360378",
					DateListed = DateTime.Parse("2025-03-14 14:52:59.8122214"),
					DiscountedPrice = 827.99m
				}
			);

			// Add price history seed data
			builder.Entity<ProductPriceHistory>().HasData(
				new ProductPriceHistory {
					Id = 1,
					ProductId = 1,
					Price = 7.50m,
					Timestamp = DateTime.Now
				},
				new ProductPriceHistory {
					Id = 2,
					ProductId = 2,
					Price = 4.87m,
					Timestamp = DateTime.Now
				},
				new ProductPriceHistory {
					Id = 3,
					ProductId = 3,
					Price = 3.85m,
					Timestamp = DateTime.Now
				},
				new ProductPriceHistory {
					Id = 4,
					ProductId = 4,
					Price = 280.17m,
					Timestamp = DateTime.Now
				},
				new ProductPriceHistory {
					Id = 5,
					ProductId = 5,
					Price = 827.99m,
					Timestamp = DateTime.Now
				},
				new ProductPriceHistory {
					Id = 26,
					ProductId = 1,
					Price = 11.50m,
					Timestamp = DateTime.Now.AddDays(-18)
				},
				new ProductPriceHistory {
					Id = 27,
					ProductId = 2,
					Price = 4.87m,
					Timestamp = DateTime.Now.AddDays(-1)
				},
				new ProductPriceHistory {
					Id = 28,
					ProductId = 2,
					Price = 5.99m,
					Timestamp = DateTime.Now.AddDays(-3)
				},
				new ProductPriceHistory {
					Id = 29,
					ProductId = 2,
					Price = 5.99m,
					Timestamp = DateTime.Now.AddDays(-7)
				},
				new ProductPriceHistory {
					Id = 30,
					ProductId = 2,
					Price = 6.50m,
					Timestamp = DateTime.Now.AddDays(-14)
				},
				new ProductPriceHistory {
					Id = 31,
					ProductId = 2,
					Price = 6.50m,
					Timestamp = DateTime.Now.AddDays(-21)
				},
				new ProductPriceHistory {
					Id = 32,
					ProductId = 2,
					Price = 5.99m,
					Timestamp = DateTime.Now.AddDays(-28)
				},
				new ProductPriceHistory {
					Id = 33,
					ProductId = 2,
					Price = 5.99m,
					Timestamp = DateTime.Now.AddDays(-35)
				},
				new ProductPriceHistory {
					Id = 34,
					ProductId = 2,
					Price = 4.99m,
					Timestamp = DateTime.Now.AddDays(-42)
				},
				new ProductPriceHistory {
					Id = 35,
					ProductId = 2,
					Price = 4.99m,
					Timestamp = DateTime.Now.AddDays(-49)
				},
				new ProductPriceHistory {
					Id = 36,
					ProductId = 2,
					Price = 5.50m,
					Timestamp = DateTime.Now.AddDays(-56)
				},
				new ProductPriceHistory {
					Id = 37,
					ProductId = 2,
					Price = 5.99m,
					Timestamp = DateTime.Now.AddDays(-63)
				},
				new ProductPriceHistory {
					Id = 38,
					ProductId = 2,
					Price = 6.99m,
					Timestamp = DateTime.Now.AddDays(-70)
				},
				new ProductPriceHistory {
					Id = 39,
					ProductId = 2,
					Price = 6.99m,
					Timestamp = DateTime.Now.AddDays(-77)
				},
				new ProductPriceHistory {
					Id = 40,
					ProductId = 2,
					Price = 5.99m,
					Timestamp = DateTime.Now.AddDays(-84)
				},
				new ProductPriceHistory {
					Id = 41,
					ProductId = 2,
					Price = 5.99m,
					Timestamp = DateTime.Now.AddDays(-91)
				}
			);
		}
	}
}