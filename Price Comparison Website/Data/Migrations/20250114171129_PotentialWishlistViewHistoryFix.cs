using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Price_Comparison_Website.Data.Migrations
{
    /// <inheritdoc />
    public partial class PotentialWishlistViewHistoryFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserViewingHistory");

            migrationBuilder.DropTable(
                name: "UserWishList");

            migrationBuilder.CreateTable(
                name: "UserViewingHistories",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserViewingHistories", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_UserViewingHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserViewingHistories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWishLists",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWishLists", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_UserWishLists_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWishLists_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserViewingHistories_ProductId",
                table: "UserViewingHistories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWishLists_ProductId",
                table: "UserWishLists",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserViewingHistories");

            migrationBuilder.DropTable(
                name: "UserWishLists");

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
    }
}
