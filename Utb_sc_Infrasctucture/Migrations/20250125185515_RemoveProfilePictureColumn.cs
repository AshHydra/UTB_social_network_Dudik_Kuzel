using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utb_sc_Infrasctructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProfilePictureColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePictureUrl",
                table: "User",
                newName: "ProfilePicturePath");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "efdb372e-a2aa-46c6-9e9e-e8adbe282598", "AQAAAAIAAYagAAAAEB6J7edzYGyP5Hx+cs2MI8vsSusp0QIpETEAQ2vxTZDQwTqYd2HGCS9m7XF6U3dlkw==", "5706730c-8161-4e76-b3a4-a521a9361dd6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePicturePath",
                table: "User",
                newName: "ProfilePictureUrl");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "65322f78-552b-4487-b4de-9e49edde1400", "AQAAAAIAAYagAAAAEGDeoYM2eDNNnCYbl7/QSYDp+njYmDCmwzV0722so/T9Yss2U+xQAeL5okveWHcdTQ==", "efa11bbb-211a-4a08-abf3-f08f87dad71c" });
        }
    }
}
