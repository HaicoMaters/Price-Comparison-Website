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
		public DbSet<LoginActivity> LoginActivities { get; set; }
		public DbSet<ScraperStatus> ScraperStatus {get; set;}

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
				}
			);

			// Vendors
			builder.Entity<Vendor>().HasData(
				new Vendor { 
					VendorId = 1, 
					Name = "Amazon", 
					VendorUrl = "https://www.amazon.co.uk", 
					VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/a/a9/Amazon_logo.svg"
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
				}
			);
		}
	}
}