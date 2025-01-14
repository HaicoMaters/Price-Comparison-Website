using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateListingRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceListings_Products_ProductId1",
                table: "PriceListings");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceListings_Vendors_VendorId1",
                table: "PriceListings");

            migrationBuilder.DropIndex(
                name: "IX_PriceListings_ProductId1",
                table: "PriceListings");

            migrationBuilder.DropIndex(
                name: "IX_PriceListings_VendorId1",
                table: "PriceListings");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "PriceListings");

            migrationBuilder.DropColumn(
                name: "VendorId1",
                table: "PriceListings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "PriceListings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorId1",
                table: "PriceListings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceListings_ProductId1",
                table: "PriceListings",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListings_VendorId1",
                table: "PriceListings",
                column: "VendorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListings_Products_ProductId1",
                table: "PriceListings",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListings_Vendors_VendorId1",
                table: "PriceListings",
                column: "VendorId1",
                principalTable: "Vendors",
                principalColumn: "VendorId");
        }
    }
}
