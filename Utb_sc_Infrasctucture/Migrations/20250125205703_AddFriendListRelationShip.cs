using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utb_sc_Infrasctructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendListRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "FriendLists");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a6290ce1-a03f-43bb-b23a-864d717e58d2", "AQAAAAIAAYagAAAAED4c0xmtvc8qT9OkEiEErWfotHChfOLEjiv0bwF++LYxiErNQudX9OwIlJgCm+EXSA==", "bba93342-b742-4c60-8001-1d015434c247" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "FriendLists",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "efdb372e-a2aa-46c6-9e9e-e8adbe282598", "AQAAAAIAAYagAAAAEB6J7edzYGyP5Hx+cs2MI8vsSusp0QIpETEAQ2vxTZDQwTqYd2HGCS9m7XF6U3dlkw==", "5706730c-8161-4e76-b3a4-a521a9361dd6" });
        }
    }
}
