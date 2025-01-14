using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateListingRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "IX_PriceListings_ProductId",
                table: "PriceListings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListings_ProductId1",
                table: "PriceListings",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListings_VendorId",
                table: "PriceListings",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListings_VendorId1",
                table: "PriceListings",
                column: "VendorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListings_Products_ProductId",
                table: "PriceListings",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListings_Products_ProductId1",
                table: "PriceListings",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListings_Vendors_VendorId",
                table: "PriceListings",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListings_Vendors_VendorId1",
                table: "PriceListings",
                column: "VendorId1",
                principalTable: "Vendors",
                principalColumn: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceListings_Products_ProductId",
                table: "PriceListings");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceListings_Products_ProductId1",
                table: "PriceListings");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceListings_Vendors_VendorId",
                table: "PriceListings");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceListings_Vendors_VendorId1",
                table: "PriceListings");

            migrationBuilder.DropIndex(
                name: "IX_PriceListings_ProductId",
                table: "PriceListings");

            migrationBuilder.DropIndex(
                name: "IX_PriceListings_ProductId1",
                table: "PriceListings");

            migrationBuilder.DropIndex(
                name: "IX_PriceListings_VendorId",
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
    }
}
