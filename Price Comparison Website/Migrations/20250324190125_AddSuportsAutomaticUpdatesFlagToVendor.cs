using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Price_Comparison_Website.Migrations
{
    /// <inheritdoc />
    public partial class AddSuportsAutomaticUpdatesFlagToVendor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SupportsAutomaticUpdates",
                table: "Vendors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Vendors",
                keyColumn: "VendorId",
                keyValue: 1,
                column: "SupportsAutomaticUpdates",
                value: false);

            migrationBuilder.UpdateData(
                table: "Vendors",
                keyColumn: "VendorId",
                keyValue: 2,
                column: "SupportsAutomaticUpdates",
                value: false);

            migrationBuilder.UpdateData(
                table: "Vendors",
                keyColumn: "VendorId",
                keyValue: 11,
                column: "SupportsAutomaticUpdates",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupportsAutomaticUpdates",
                table: "Vendors");
        }
    }
}
