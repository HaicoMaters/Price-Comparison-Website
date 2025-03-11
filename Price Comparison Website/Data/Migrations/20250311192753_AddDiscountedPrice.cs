using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountedPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountedPrice",
                table: "PriceListings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountedPrice",
                table: "PriceListings",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

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
    }
}
