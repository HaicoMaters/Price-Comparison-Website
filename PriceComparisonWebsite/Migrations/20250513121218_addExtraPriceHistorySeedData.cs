using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PriceComparisonWebsite.Migrations
{
    /// <inheritdoc />
    public partial class addExtraPriceHistorySeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Timestamp",
                value: new DateTime(2025, 5, 13, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9412));

            migrationBuilder.UpdateData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Timestamp",
                value: new DateTime(2025, 5, 13, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9465));

            migrationBuilder.UpdateData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Timestamp",
                value: new DateTime(2025, 5, 13, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9469));

            migrationBuilder.UpdateData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Timestamp",
                value: new DateTime(2025, 5, 13, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9473));

            migrationBuilder.UpdateData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Timestamp",
                value: new DateTime(2025, 5, 13, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9477));

            migrationBuilder.InsertData(
                table: "ProductPriceHistories",
                columns: new[] { "Id", "Price", "ProductId", "Timestamp" },
                values: new object[,]
                {
                    { 26, 11.50m, 1, new DateTime(2025, 4, 25, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9480) },
                    { 27, 4.87m, 2, new DateTime(2025, 5, 12, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9486) },
                    { 28, 5.99m, 2, new DateTime(2025, 5, 10, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9491) },
                    { 29, 5.99m, 2, new DateTime(2025, 5, 6, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9495) },
                    { 30, 6.50m, 2, new DateTime(2025, 4, 29, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9499) },
                    { 31, 6.50m, 2, new DateTime(2025, 4, 22, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9503) },
                    { 32, 5.99m, 2, new DateTime(2025, 4, 15, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9508) },
                    { 33, 5.99m, 2, new DateTime(2025, 4, 8, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9512) },
                    { 34, 4.99m, 2, new DateTime(2025, 4, 1, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9516) },
                    { 35, 4.99m, 2, new DateTime(2025, 3, 25, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9519) },
                    { 36, 5.50m, 2, new DateTime(2025, 3, 18, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9522) },
                    { 37, 5.99m, 2, new DateTime(2025, 3, 11, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9525) },
                    { 38, 6.99m, 2, new DateTime(2025, 3, 4, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9528) },
                    { 39, 6.99m, 2, new DateTime(2025, 2, 25, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9531) },
                    { 40, 5.99m, 2, new DateTime(2025, 2, 18, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9535) },
                    { 41, 5.99m, 2, new DateTime(2025, 2, 11, 13, 12, 16, 457, DateTimeKind.Local).AddTicks(9538) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.UpdateData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Timestamp",
                value: new DateTime(2025, 3, 14, 14, 52, 37, 413, DateTimeKind.Unspecified).AddTicks(3597));

            migrationBuilder.UpdateData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Timestamp",
                value: new DateTime(2025, 3, 14, 14, 52, 45, 283, DateTimeKind.Unspecified).AddTicks(3811));

            migrationBuilder.UpdateData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Timestamp",
                value: new DateTime(2025, 3, 14, 14, 52, 49, 664, DateTimeKind.Unspecified).AddTicks(9873));

            migrationBuilder.UpdateData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 4,
                column: "Timestamp",
                value: new DateTime(2025, 3, 14, 14, 52, 57, 812, DateTimeKind.Unspecified).AddTicks(2214));

            migrationBuilder.UpdateData(
                table: "ProductPriceHistories",
                keyColumn: "Id",
                keyValue: 5,
                column: "Timestamp",
                value: new DateTime(2025, 3, 14, 14, 52, 59, 812, DateTimeKind.Unspecified).AddTicks(2214));
        }
    }
}
