using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PriceComparisonWebsite.Migrations
{
    /// <inheritdoc />
    public partial class addedPriceHistorySeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductPriceHistories",
                columns: new[] { "Id", "Price", "ProductId", "Timestamp" },
                values: new object[,]
                {
                    { 1, 7.50m, 1, new DateTime(2025, 3, 14, 14, 52, 37, 413, DateTimeKind.Unspecified).AddTicks(3597) },
                    { 2, 4.87m, 2, new DateTime(2025, 3, 14, 14, 52, 45, 283, DateTimeKind.Unspecified).AddTicks(3811) },
                    { 3, 3.85m, 3, new DateTime(2025, 3, 14, 14, 52, 49, 664, DateTimeKind.Unspecified).AddTicks(9873) },
                    { 4, 280.17m, 4, new DateTime(2025, 3, 14, 14, 52, 57, 812, DateTimeKind.Unspecified).AddTicks(2214) },
                    { 5, 827.99m, 5, new DateTime(2025, 3, 14, 14, 52, 59, 812, DateTimeKind.Unspecified).AddTicks(2214) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
