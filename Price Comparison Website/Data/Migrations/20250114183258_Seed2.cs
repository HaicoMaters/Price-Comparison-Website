using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class Seed2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
