using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateListingRelationshi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_ApplicationUserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_ApplicationUserId1",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ApplicationUserId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_ApplicationUserId1",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "UserViewingHistory",
                columns: table => new
                {
                    ApplicationUser1Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ViewingHistoryProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserViewingHistory", x => new { x.ApplicationUser1Id, x.ViewingHistoryProductId });
                    table.ForeignKey(
                        name: "FK_UserViewingHistory_AspNetUsers_ApplicationUser1Id",
                        column: x => x.ApplicationUser1Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserViewingHistory_Products_ViewingHistoryProductId",
                        column: x => x.ViewingHistoryProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWishList",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WishListProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWishList", x => new { x.ApplicationUserId, x.WishListProductId });
                    table.ForeignKey(
                        name: "FK_UserWishList_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWishList_Products_WishListProductId",
                        column: x => x.WishListProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserViewingHistory_ViewingHistoryProductId",
                table: "UserViewingHistory",
                column: "ViewingHistoryProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWishList_WishListProductId",
                table: "UserWishList",
                column: "WishListProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserViewingHistory");

            migrationBuilder.DropTable(
                name: "UserWishList");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Products",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "Products",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ApplicationUserId",
                table: "Products",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ApplicationUserId1",
                table: "Products",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_ApplicationUserId",
                table: "Products",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_ApplicationUserId1",
                table: "Products",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
