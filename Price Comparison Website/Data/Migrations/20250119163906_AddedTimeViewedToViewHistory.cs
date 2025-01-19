using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedTimeViewedToViewHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastViewed",
                table: "UserViewingHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 1,
                column: "DateListed",
                value: new DateTime(2025, 1, 19, 16, 39, 5, 396, DateTimeKind.Local).AddTicks(1563));

            migrationBuilder.UpdateData(
                table: "PriceListings",
                keyColumn: "PriceListingId",
                keyValue: 2,
                column: "DateListed",
                value: new DateTime(2025, 1, 19, 16, 39, 5, 396, DateTimeKind.Local).AddTicks(1606));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastViewed",
                table: "UserViewingHistories");

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
    }
}
