using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBackDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 1, 16, 23, 28, 59, 584, DateTimeKind.Local).AddTicks(1316));

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 2,
                column: "DateListed",
                value: new DateTime(2025, 1, 16, 23, 28, 59, 584, DateTimeKind.Local).AddTicks(1520));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 1, 16, 23, 0, 16, 366, DateTimeKind.Local).AddTicks(3661));

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 2,
                column: "DateListed",
                value: new DateTime(2025, 1, 16, 23, 0, 16, 366, DateTimeKind.Local).AddTicks(3702));
        }
    }
}
