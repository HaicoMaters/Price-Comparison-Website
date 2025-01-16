﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Price_Comparison_Website.Data;

#nullable disable

namespace Price_Comparison_Website.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderKey")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            CategoryId = 1,
                            Name = "Electronics"
                        },
                        new
                        {
                            CategoryId = 2,
                            Name = "Tools"
                        },
                        new
                        {
                            CategoryId = 3,
                            Name = "Clothing"
                        },
                        new
                        {
                            CategoryId = 4,
                            Name = "Food"
                        },
                        new
                        {
                            CategoryId = 5,
                            Name = "Pet Items"
                        },
                        new
                        {
                            CategoryId = 6,
                            Name = "Toys"
                        },
                        new
                        {
                            CategoryId = 7,
                            Name = "Stationary"
                        },
                        new
                        {
                            CategoryId = 8,
                            Name = "Drinks"
                        },
                        new
                        {
                            CategoryId = 9,
                            Name = "Games"
                        },
                        new
                        {
                            CategoryId = 10,
                            Name = "Books"
                        },
                        new
                        {
                            CategoryId = 11,
                            Name = "Furniture"
                        },
                        new
                        {
                            CategoryId = 12,
                            Name = "Beauty"
                        },
                        new
                        {
                            CategoryId = 13,
                            Name = "Sports"
                        },
                        new
                        {
                            CategoryId = 14,
                            Name = "Health"
                        },
                        new
                        {
                            CategoryId = 15,
                            Name = "Music"
                        },
                        new
                        {
                            CategoryId = 16,
                            Name = "Movies"
                        },
                        new
                        {
                            CategoryId = 17,
                            Name = "Travel"
                        },
                        new
                        {
                            CategoryId = 18,
                            Name = "Gifts"
                        });
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.PriceListing", b =>
                {
                    b.Property<int>("PriceListingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PriceListingId"));

                    b.Property<DateTime?>("DateListed")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("PurchaseUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VendorId")
                        .HasColumnType("int");

                    b.HasKey("PriceListingId");

                    b.HasIndex("ProductId");

                    b.HasIndex("VendorId");

                    b.ToTable("PriceListings");

                    b.HasData(
                        new
                        {
                            PriceListingId = 1,
                            DateListed = new DateTime(2025, 1, 16, 23, 28, 59, 584, DateTimeKind.Local).AddTicks(1316),
                            Price = 99.00m,
                            ProductId = 1,
                            PurchaseUrl = "https://www.amazon.com/Stealth-Wireless-Multiplatform-Amplified-Headset-Nintendo/dp/B0CYWFH5Y9/ref=sr_1_5?_encoding=UTF8&content-id=amzn1.sym.12129333-2117-4490-9c17-6d31baf0582a&dib=eyJ2IjoiMSJ9.VGoru17L34M5u4AqX0EqqypmcGNBhQxULfjaHbYVNmd0PXtUKoq0IryVhEe8Avp17c7W4F1avbJAkvdvMH3jBAvS1y-h85YgufTd1_YFIBFyMR3ugPGW3V_AdDjgteUFyhz_Eez0nfm7auWFQlzkPy2RTQwsDVHjaVrtwgAkM3xC_LGBLXpf8WBiOQfNuzqJezm6DyoWKDfnAMQK88unx_KwWs3-xqdFcuBtzcNb5QU.kSL29Wzha8iyoHFr3XL-ZPQOSlLNXOyLBsq88fLMoBg&dib_tag=se&keywords=gaming+headsets&pd_rd_r=2d291fd7-dfbe-4e94-a175-382f3c52d742&pd_rd_w=WK5FS&pd_rd_wg=SgrUc&pf_rd_p=12129333-2117-4490-9c17-6d31baf0582a&pf_rd_r=K2AVFG3GKGVRSJCX847Q&qid=1737064698&sr=8-5&th=1",
                            VendorId = 1
                        },
                        new
                        {
                            PriceListingId = 2,
                            DateListed = new DateTime(2025, 1, 16, 23, 28, 59, 584, DateTimeKind.Local).AddTicks(1520),
                            Price = 190.36m,
                            ProductId = 1,
                            PurchaseUrl = "https://www.ebay.co.uk/itm/286226735878",
                            VendorId = 2
                        });
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            CategoryId = 1,
                            Description = "Multiplatform, Low-latency 2.4GHz Wireless + Bluetooth 5.2\r\nBest-in-Class 80-Hour Battery Life with Quick Charge\r\nQuickSwitch Button for Seamless Wireless to Bluetooth switching\r\nFlip-to-Mute Mic with A.I.-Based Noise Reduction\r\nPowerful, 50mm Nanoclear Drivers for Vibrant Spatial Audio\r\nSwarm II Desktop & Mobile App with Advanced 10-Band EQ\r\nMappable Wheel & Mode Button for Customizable Functions",
                            ImageUrl = "https://m.media-amazon.com/images/I/71kECPK7CXL._AC_SL1500_.jpg",
                            Name = "Turtle Beach Stealth 600 Gen 3 Wireless Multiplatform Amplified Gaming Headset"
                        });
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.UserViewingHistory", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("UserViewingHistories");
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.UserWishList", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("UserId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("UserWishLists");
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.Vendor", b =>
                {
                    b.Property<int>("VendorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VendorId"));

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VendorLogoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VendorUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VendorId");

                    b.ToTable("Vendors");

                    b.HasData(
                        new
                        {
                            VendorId = 1,
                            Name = "Amazon",
                            VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/a/a9/Amazon_logo.svg",
                            VendorUrl = "https://www.amazon.com"
                        },
                        new
                        {
                            VendorId = 2,
                            Name = "eBay",
                            VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/1/1b/EBay_logo.svg",
                            VendorUrl = "https://www.ebay.com"
                        },
                        new
                        {
                            VendorId = 3,
                            Name = "Best Buy",
                            VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/2/2f/Best_Buy_Logo.svg",
                            VendorUrl = "https://www.bestbuy.com"
                        },
                        new
                        {
                            VendorId = 4,
                            Name = "Target",
                            VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/9/9a/Target_logo.svg",
                            VendorUrl = "https://www.target.com"
                        },
                        new
                        {
                            VendorId = 5,
                            Name = "Home Depot",
                            VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/5/5f/TheHomeDepot.svg",
                            VendorUrl = "https://www.homedepot.com"
                        },
                        new
                        {
                            VendorId = 6,
                            Name = "Ikea",
                            VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/8/8b/Ikea_logo.svg",
                            VendorUrl = "https://www.ikea.com"
                        },
                        new
                        {
                            VendorId = 7,
                            Name = "AliExpress",
                            VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/f/f9/AliExpress_logo.svg",
                            VendorUrl = "https://www.aliexpress.com"
                        },
                        new
                        {
                            VendorId = 8,
                            Name = "Etsy",
                            VendorLogoUrl = "https://upload.wikimedia.org/wikipedia/commons/8/82/Etsy_logo.svg",
                            VendorUrl = "https://www.etsy.com"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Price_Comparison_Website.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Price_Comparison_Website.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Price_Comparison_Website.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Price_Comparison_Website.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.PriceListing", b =>
                {
                    b.HasOne("Price_Comparison_Website.Models.Product", "Product")
                        .WithMany("PriceListings")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Price_Comparison_Website.Models.Vendor", "Vendor")
                        .WithMany("PriceListings")
                        .HasForeignKey("VendorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.Product", b =>
                {
                    b.HasOne("Price_Comparison_Website.Models.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.UserViewingHistory", b =>
                {
                    b.HasOne("Price_Comparison_Website.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Price_Comparison_Website.Models.ApplicationUser", "User")
                        .WithMany("ViewingHistory")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.UserWishList", b =>
                {
                    b.HasOne("Price_Comparison_Website.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Price_Comparison_Website.Models.ApplicationUser", "User")
                        .WithMany("WishList")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.ApplicationUser", b =>
                {
                    b.Navigation("ViewingHistory");

                    b.Navigation("WishList");
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.Product", b =>
                {
                    b.Navigation("PriceListings");
                });

            modelBuilder.Entity("Price_Comparison_Website.Models.Vendor", b =>
                {
                    b.Navigation("PriceListings");
                });
#pragma warning restore 612, 618
        }
    }
}
