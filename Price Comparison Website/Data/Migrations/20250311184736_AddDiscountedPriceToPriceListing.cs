using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountedPriceToPriceListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "PriceListings",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 1,
                columns: new[] { "DateListed", "DiscountedPrice" },
                values: new object[] { new DateTime(2025, 3, 11, 18, 47, 35, 653, DateTimeKind.Local).AddTicks(5434), null });

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 2,
                columns: new[] { "DateListed", "DiscountedPrice" },
                values: new object[] { new DateTime(2025, 3, 11, 18, 47, 35, 653, DateTimeKind.Local).AddTicks(5485), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "PriceListings");

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 3, 11, 18, 24, 56, 798, DateTimeKind.Local).AddTicks(5121));

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 2,
                column: "DateListed",
                value: new DateTime(2025, 3, 11, 18, 24, 56, 798, DateTimeKind.Local).AddTicks(5181));
        }
    }
}
