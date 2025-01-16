using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class Seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 1,
                columns: new[] { "DateListed", "PurchaseUrl" },
                values: new object[] { new DateTime(2025, 1, 16, 23, 0, 16, 366, DateTimeKind.Local).AddTicks(3661), "https://www.amazon.com/Stealth-Wireless-Multiplatform-Amplified-Headset-Nintendo/dp/B0CYWFH5Y9/ref=sr_1_5?_encoding=UTF8&content-id=amzn1.sym.12129333-2117-4490-9c17-6d31baf0582a&dib=eyJ2IjoiMSJ9.VGoru17L34M5u4AqX0EqqypmcGNBhQxULfjaHbYVNmd0PXtUKoq0IryVhEe8Avp17c7W4F1avbJAkvdvMH3jBAvS1y-h85YgufTd1_YFIBFyMR3ugPGW3V_AdDjgteUFyhz_Eez0nfm7auWFQlzkPy2RTQwsDVHjaVrtwgAkM3xC_LGBLXpf8WBiOQfNuzqJezm6DyoWKDfnAMQK88unx_KwWs3-xqdFcuBtzcNb5QU.kSL29Wzha8iyoHFr3XL-ZPQOSlLNXOyLBsq88fLMoBg&dib_tag=se&keywords=gaming+headsets&pd_rd_r=2d291fd7-dfbe-4e94-a175-382f3c52d742&pd_rd_w=WK5FS&pd_rd_wg=SgrUc&pf_rd_p=12129333-2117-4490-9c17-6d31baf0582a&pf_rd_r=K2AVFG3GKGVRSJCX847Q&qid=1737064698&sr=8-5&th=1" });

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 2,
                columns: new[] { "DateListed", "PurchaseUrl" },
                values: new object[] { new DateTime(2025, 1, 16, 23, 0, 16, 366, DateTimeKind.Local).AddTicks(3702), "https://www.ebay.co.uk/itm/286226735878" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Multiplatform, Low-latency 2.4GHz Wireless + Bluetooth 5.2\r\nBest-in-Class 80-Hour Battery Life with Quick Charge\r\nQuickSwitch Button for Seamless Wireless to Bluetooth switching\r\nFlip-to-Mute Mic with A.I.-Based Noise Reduction\r\nPowerful, 50mm Nanoclear Drivers for Vibrant Spatial Audio\r\nSwarm II Desktop & Mobile App with Advanced 10-Band EQ\r\nMappable Wheel & Mode Button for Customizable Functions", "Turtle Beach Stealth 600 Gen 3 Wireless Multiplatform Amplified Gaming Headset" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 1,
                columns: new[] { "DateListed", "PurchaseUrl" },
                values: new object[] { new DateTime(2025, 1, 16, 22, 49, 7, 202, DateTimeKind.Local).AddTicks(547), "https://www.amazon.com/Stealth-Wireless-Multiplatform-Amplified-Headset-Nintendo/dp/B0CYWFH5Y9" });

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 2,
                columns: new[] { "DateListed", "PurchaseUrl" },
                values: new object[] { new DateTime(2025, 1, 16, 22, 49, 7, 202, DateTimeKind.Local).AddTicks(592), "https://www.ebay.com/itm/286226735878" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Low-latency wireless with 80-hour battery life.", "Turtle Beach Stealth 600 Gen 3 Wireless Gaming Headset" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Description", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 2, 2, "18V Lithium-Ion Cordless Drill with 2 Batteries.", "https://m.media-amazon.com/images/I/71TkKfUE5tL._AC_SL1500_.jpg", "Makita Cordless Drill" },
                    { 3, 3, "Stylish genuine leather jacket for all seasons.", "https://m.media-amazon.com/images/I/71hL5OdDfkL._AC_UL1500_.jpg", "Men's Leather Jacket" },
                    { 4, 4, "Premium Arabica coffee beans for coffee lovers.", "https://m.media-amazon.com/images/I/81yK1I2v+7L._AC_SL1500_.jpg", "Gourmet Coffee Beans" },
                    { 5, 5, "Comfortable bed for your pet to sleep in.", "https://m.media-amazon.com/images/I/91tFlLWs9bL._AC_SL1500_.jpg", "Pet Dog Bed" },
                    { 6, 6, "Build your own Star Wars ships with this LEGO set.", "https://m.media-amazon.com/images/I/91y4VNCpdVL._AC_SL1500_.jpg", "LEGO Star Wars Set" },
                    { 7, 7, "Pack of 20 ballpoint pens, medium point.", "https://m.media-amazon.com/images/I/71rlE9Hr8dL._AC_SL1500_.jpg", "BIC Round Stic Pens" },
                    { 8, 8, "Refreshing Coca-Cola drink in a 6-pack of cans.", "https://m.media-amazon.com/images/I/71p7DQ0wwHL._AC_SL1500_.jpg", "Coca-Cola Soda" },
                    { 9, 9, "Next-gen gaming console with ultra-high-speed SSD.", "https://m.media-amazon.com/images/I/91hBYiq0jQL._AC_SL1500_.jpg", "PlayStation 5 Console" },
                    { 10, 10, "The complete set of 7 books in the Harry Potter series.", "https://m.media-amazon.com/images/I/91e5U7ADuyL._AC_SL1500_.jpg", "Harry Potter Book Set" },
                    { 11, 11, "Comfortable and stylish 3-piece sofa set for your home.", "https://m.media-amazon.com/images/I/81m82HyH2yL._AC_SL1500_.jpg", "Sofa Set" },
                    { 12, 12, "Smooth and even foundation with SPF 15 protection.", "https://m.media-amazon.com/images/I/91sLD3u7JLL._AC_SL1500_.jpg", "Maybelline Foundation" },
                    { 13, 13, "Non-slip, eco-friendly yoga mat for all levels.", "https://m.media-amazon.com/images/I/81uWqK2l+mL._AC_SL1500_.jpg", "Yoga Mat" },
                    { 14, 14, "Track your fitness progress with this advanced heart rate monitor.", "https://m.media-amazon.com/images/I/91uy8RZPqZL._AC_SL1500_.jpg", "Heart Rate Monitor Watch" },
                    { 15, 15, "Premium guitar picks in different thicknesses.", "https://m.media-amazon.com/images/I/91m4ZCugAkL._AC_SL1500_.jpg", "Guitar Pick Set" },
                    { 16, 16, "Display your favorite movie posters in style with this frame.", "https://m.media-amazon.com/images/I/81IjAgu10mL._AC_SL1500_.jpg", "Movie Poster Frame" },
                    { 17, 17, "Durable and spacious backpack for all your travel essentials.", "https://m.media-amazon.com/images/I/91b9RpqOeCL._AC_SL1500_.jpg", "Travel Backpack" },
                    { 18, 18, "Gift basket with assorted chocolates and goodies.", "https://m.media-amazon.com/images/I/81R-CmbkpLL._AC_SL1500_.jpg", "Gifting Basket" },
                    { 19, 3, "Warm and stylish winter jacket for cold weather.", "https://m.media-amazon.com/images/I/91Dzpw8iL1L._AC_SL1500_.jpg", "Men's Winter Jacket" },
                    { 20, 1, "Waterproof and portable Bluetooth speaker for outdoor use.", "https://m.media-amazon.com/images/I/91g2c2tKmPL._AC_SL1500_.jpg", "Portable Bluetooth Speaker" }
                });
        }
    }
}
