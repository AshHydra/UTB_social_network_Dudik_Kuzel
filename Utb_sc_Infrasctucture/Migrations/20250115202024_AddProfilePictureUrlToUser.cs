using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utb_sc_Infrasctructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureUrlToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "User",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePath",
                table: "AspNetUsers",
                type: "longtext",
                nullable: false,
                defaultValue: "/images/default.png")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "ProfilePicturePath", "SecurityStamp" },
                values: new object[] { "65322f78-552b-4487-b4de-9e49edde1400", "AQAAAAIAAYagAAAAEGDeoYM2eDNNnCYbl7/QSYDp+njYmDCmwzV0722so/T9Yss2U+xQAeL5okveWHcdTQ==", "/images/default.png", "efa11bbb-211a-4a08-abf3-f08f87dad71c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ProfilePicturePath",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6c8516c9-5ce0-48a8-bb01-3024bd7edeb7", "AQAAAAIAAYagAAAAENY5/pvFQUP0Di55LZyWCK8IvyXTzON01U3doF9CVYFFgrFkt2Ra/QyqO3RfAs/YfQ==", "188e7dd3-8341-48a3-acaf-ef8a6c333e4f" });
        }
    }
}
