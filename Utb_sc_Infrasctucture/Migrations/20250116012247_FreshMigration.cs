using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Utb_sc_Infrasctructure.Migrations
{
    /// <inheritdoc />
    public partial class FreshMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "69c41b77-b651-4985-bb56-3b783d81727c", "AQAAAAIAAYagAAAAEGE5D5KczAbSuyOr7AI7iX4lwEDQeEQ631K6R6CZ0Kg+gr8nxTvc6JC4JZCa+3I9xA==", "485151f4-12c5-4d47-9023-1407bf9c8950" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "98b618d6-0b9b-4a75-bee5-7ebc5b8d9870", "AQAAAAIAAYagAAAAEBkubO9q8XCj3cjYWojLa02q6IFKWPoBPsTiQPu2IOqOKr8GPzvuydqw/KmDoMfbBg==", "146874f2-4f07-4678-b82c-a4bc6c49daf8" });
        }
    }
}
