using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PriceComparisonWebsite.Migrations
{
    /// <inheritdoc />
    public partial class addedNewSeedDataForNewParser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "CheapestPrice", "Description", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 4, 1, 280.17m, "The AMD Ryzen 7 9700X is a high-performance processor designed for gaming and content creation. It features 8 cores and 16 threads, with a base clock speed of 3.8 GHz and a boost clock speed of up to 4.5 GHz. The processor is built on the 7nm process technology and supports DDR4 memory up to 3200 MHz. It also includes support for PCIe 4.0, allowing for faster data transfer rates with compatible devices. The Ryzen 7 9700X is compatible with AM4 motherboards and has a TDP of 105W.", "https://m.media-amazon.com/images/I/61F3ZChalyL._AC_SX679_.jpg", "AMD Ryzen 7 9700X Processor" },
                    { 5, 1, 827.99m, "Intel Core Ultra 7 256V, 16GB LPDDR5X Onboard Memory Memory / 1 TB PCIe SSD, Intel Arc Graphics 140V, Non-Touch Screen, 1920 x 1200, Windows 11 Home", "https://c1.neweggimages.com/ProductImageCompressAll60/34-360-378-03.jpg", "Acer Aspire 14 AI Copilot+ PC 14.0 Touchscreen Laptop Ultra 7 256V 16GB RAM 1TB SSD Windows 11 Home A14-52M-72FH" }
                });

            migrationBuilder.InsertData(
                table: "Vendors",
                columns: new[] { "VendorId", "Name", "SupportsAutomaticUpdates", "VendorLogoUrl", "VendorUrl" },
                values: new object[] { 3, "Newegg", false, "https://c1.neweggimages.com/WebResource/Themes/Nest/logos/Newegg_full_color_logo_RGB.SVG", "https://www.newegg.com" });

            migrationBuilder.InsertData(
                table: "PriceListings",
                columns: new[] { "PriceListingId", "DateListed", "DiscountedPrice", "Price", "ProductId", "PurchaseUrl", "VendorId" },
                values: new object[,]
                {
                    { 16, new DateTime(2025, 3, 14, 14, 52, 55, 812, DateTimeKind.Unspecified).AddTicks(2214), 319.19m, 327.59m, 4, "https://www.newegg.com/global/uk-en/amd-ryzen-7-9700x-ryzen-7-9000-series-granite-ridge-socket-am5-processor/p/N82E16819113843", 3 },
                    { 17, new DateTime(2025, 3, 14, 14, 52, 57, 812, DateTimeKind.Unspecified).AddTicks(2214), 280.17m, 298.00m, 4, "https://www.amazon.co.uk/AMD-Ryzensets-9700X-Processor-frequency/dp/B0D6NMDNNX", 1 },
                    { 18, new DateTime(2025, 3, 14, 14, 52, 59, 812, DateTimeKind.Unspecified).AddTicks(2214), 827.99m, 827.99m, 5, "https://www.newegg.com/global/uk-en/aspire-14-ai/p/N82E16834360378", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Vendors",
                keyColumn: "VendorId",
                keyValue: 3);
        }
    }
}
