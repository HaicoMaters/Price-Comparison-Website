using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 30);

            migrationBuilder.AddColumn<string>(
                name: "VendorLogoUrl",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Description", "ImageUrl", "Name" },
                values: new object[] { 53, 1, "Multiplatform, Low-latency 2.4GHz Wireless + Bluetooth 5.2\r\nBest-in-Class 80-Hour Battery Life with Quick Charge\r\nQuickSwitch Button for Seamless Wireless to Bluetooth switching\r\nFlip-to-Mute Mic with A.I.-Based Noise Reduction\r\nPowerful, 50mm Nanoclear Drivers for Vibrant Spatial Audio\r\nSwarm II Desktop & Mobile App with Advanced 10-Band EQ\r\nMappable Wheel & Mode Button for Customizable Functions", "https://m.media-amazon.com/images/I/71kECPK7CXL._AC_SL1500_.jpg", "Turtle Beach Stealth 600 Gen 3 Wireless Multiplatform Amplified Gaming Headset" });

            migrationBuilder.UpdateData(
                table: "Vendors",
                keyColumn: "VendorId",
                keyValue: 1,
                column: "VendorLogoUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/a/a9/Amazon_logo.svg");

            migrationBuilder.UpdateData(
                table: "Vendors",
                keyColumn: "VendorId",
                keyValue: 2,
                column: "VendorLogoUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/1/1b/EBay_logo.svg");

            migrationBuilder.UpdateData(
                table: "Vendors",
                keyColumn: "VendorId",
                keyValue: 3,
                column: "VendorLogoUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/2/2f/Best_Buy_Logo.svg");

            migrationBuilder.UpdateData(
                table: "Vendors",
                keyColumn: "VendorId",
                keyValue: 4,
                column: "VendorLogoUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/9/9a/Target_logo.svg");

            migrationBuilder.UpdateData(
                table: "Vendors",
                keyColumn: "VendorId",
                keyValue: 5,
                column: "VendorLogoUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/5/5f/TheHomeDepot.svg");

            migrationBuilder.UpdateData(
                table: "Vendors",
                keyColumn: "VendorId",
                keyValue: 6,
                column: "VendorLogoUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/8/8b/Ikea_logo.svg");

            migrationBuilder.UpdateData(
                table: "Vendors",
                keyColumn: "VendorId",
                keyValue: 7,
                column: "VendorLogoUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/f/f9/AliExpress_logo.svg");

            migrationBuilder.UpdateData(
                table: "Vendors",
                keyColumn: "VendorId",
                keyValue: 8,
                column: "VendorLogoUrl",
                value: "https://upload.wikimedia.org/wikipedia/commons/8/82/Etsy_logo.svg");

            migrationBuilder.InsertData(
                table: "PriceListings",
                columns: new[] { "PriceListingId", "DateListed", "Price", "ProductId", "PurchaseUrl", "VendorId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 16, 22, 20, 48, 289, DateTimeKind.Local).AddTicks(1859), 99.00m, 53, "https://www.amazon.com/Stealth-Wireless-Multiplatform-Amplified-Headset-Nintendo/dp/B0CYWFH5Y9/ref=sr_1_5?_encoding=UTF8&content-id=amzn1.sym.12129333-2117-4490-9c17-6d31baf0582a&dib=eyJ2IjoiMSJ9.VGoru17L34M5u4AqX0EqqypmcGNBhQxULfjaHbYVNmd0PXtUKoq0IryVhEe8Avp17c7W4F1avbJAkvdvMH3jBAvS1y-h85YgufTd1_YFIBFyMR3ugPGW3V_AdDjgteUFyhz_Eez0nfm7auWFQlzkPy2RTQwsDVHjaVrtwgAkM3xC_LGBLXpf8WBiOQfNuzqJezm6DyoWKDfnAMQK88unx_KwWs3-xqdFcuBtzcNb5QU.kSL29Wzha8iyoHFr3XL-ZPQOSlLNXOyLBsq88fLMoBg&dib_tag=se&keywords=gaming+headsets&pd_rd_r=2d291fd7-dfbe-4e94-a175-382f3c52d742&pd_rd_w=WK5FS&pd_rd_wg=SgrUc&pf_rd_p=12129333-2117-4490-9c17-6d31baf0582a&pf_rd_r=K2AVFG3GKGVRSJCX847Q&qid=1737064698&sr=8-5&th=1", 1 },
                    { 2, new DateTime(2025, 1, 16, 22, 20, 48, 289, DateTimeKind.Local).AddTicks(1906), 190.36m, 53, "https://www.ebay.co.uk/itm/286226735878", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 53);

            migrationBuilder.DropColumn(
                name: "VendorLogoUrl",
                table: "Vendors");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Description", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, 1, "A high-performance laptop", "laptop.jpg", "Laptop" },
                    { 2, 1, "Latest smartphone model", "smartphone.jpg", "Smartphone" },
                    { 3, 1, "Portable tablet for all your needs", "tablet.jpg", "Tablet" },
                    { 4, 2, "A durable hammer for all your needs", "hammer.jpg", "Hammer" },
                    { 5, 2, "Comprehensive screwdriver set", "screwdriverset.jpg", "Screwdriver Set" },
                    { 6, 3, "Cotton T-shirt in various colors", "tshirt.jpg", "T-shirt" },
                    { 7, 3, "Stylish denim jeans", "jeans.jpg", "Jeans" },
                    { 8, 3, "Warm winter jacket", "jacket.jpg", "Jacket" },
                    { 9, 4, "Ready-to-eat canned soup", "cannedsoup.jpg", "Canned Soup" },
                    { 10, 5, "A squeaky dog toy", "dogtoy.jpg", "Dog Toy" },
                    { 11, 5, "Healthy food for your cat", "catfood.jpg", "Cat Food" },
                    { 12, 6, "Colorful building blocks for kids", "blocks.jpg", "Building Blocks" },
                    { 13, 6, "A detailed action figure", "actionfigure.jpg", "Action Figure" },
                    { 14, 7, "A spiral-bound notebook", "notebook.jpg", "Notebook" },
                    { 15, 7, "High-quality pens in a set", "penset.jpg", "Pen Set" },
                    { 16, 8, "Freshly squeezed orange juice", "orangejuice.jpg", "Orange Juice" },
                    { 17, 8, "Refreshing sparkling water", "sparklingwater.jpg", "Sparkling Water" },
                    { 18, 8, "Delicious milk chocolate bar", "chocolatebar.jpg", "Chocolate Bar" },
                    { 19, 9, "Wooden chess set", "chessset.jpg", "Chess Set" },
                    { 20, 10, "A thrilling new novel", "novel.jpg", "Novel" },
                    { 21, 10, "An insightful science book", "sciencebook.jpg", "Science Book" },
                    { 22, 10, "A cookbook full of delicious recipes", "cookbook.jpg", "Cookbook" },
                    { 23, 11, "A modern coffee table", "coffeetable.jpg", "Coffee Table" },
                    { 24, 11, "Comfortable 3-seater sofa", "sofa.jpg", "Sofa" },
                    { 25, 12, "Shampoo for all hair types", "shampoo.jpg", "Shampoo" },
                    { 26, 13, "High-quality football", "football.jpg", "Football" },
                    { 27, 13, "Wooden baseball bat", "baseballbat.jpg", "Baseball Bat" },
                    { 28, 14, "Multivitamins for daily health", "vitamins.jpg", "Vitamins" },
                    { 29, 14, "Whey protein powder", "proteinpowder.jpg", "Protein Powder" },
                    { 30, 14, "Non-slip yoga mat", "yogamat.jpg", "Yoga Mat" }
                });
        }
    }
}
