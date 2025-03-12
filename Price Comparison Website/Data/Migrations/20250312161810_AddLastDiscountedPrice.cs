using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLastDiscountedPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LastCheapestPrice",
                table: "UserWishLists",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 3, 12, 16, 18, 9, 137, DateTimeKind.Local).AddTicks(3075));

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 2,
                column: "DateListed",
                value: new DateTime(2025, 3, 12, 16, 18, 9, 137, DateTimeKind.Local).AddTicks(3138));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "CheapestPrice",
                value: 99.00m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastCheapestPrice",
                table: "UserWishLists");

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 3, 12, 15, 48, 34, 969, DateTimeKind.Local).AddTicks(7004));

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 2,
                column: "DateListed",
                value: new DateTime(2025, 3, 12, 15, 48, 34, 969, DateTimeKind.Local).AddTicks(7055));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1,
                column: "CheapestPrice",
                value: 0m);
        }
    }
}
