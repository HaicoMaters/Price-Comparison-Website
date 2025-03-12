using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class CheapestPriceToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CheapestPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 1,
                columns: new[] { "DateListed", "DiscountedPrice" },
                values: new object[] { new DateTime(2025, 3, 12, 15, 48, 34, 969, DateTimeKind.Local).AddTicks(7004), 99.00m });

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 2,
                columns: new[] { "DateListed", "DiscountedPrice" },
                values: new object[] { new DateTime(2025, 3, 12, 15, 48, 34, 969, DateTimeKind.Local).AddTicks(7055), 110.21m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "CheapestPrice",
                value: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheapestPrice",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 1,
                columns: new[] { "DateListed", "DiscountedPrice" },
                values: new object[] { new DateTime(2025, 3, 11, 19, 27, 52, 379, DateTimeKind.Local).AddTicks(7424), 0m });

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 2,
                columns: new[] { "DateListed", "DiscountedPrice" },
                values: new object[] { new DateTime(2025, 3, 11, 19, 27, 52, 379, DateTimeKind.Local).AddTicks(7476), 0m });
        }
    }
}
