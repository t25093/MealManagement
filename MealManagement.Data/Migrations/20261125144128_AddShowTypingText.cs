using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShowTypingText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // משתמשות קיימות: גלוי, כמו שהיה עד עכשיו
            migrationBuilder.AddColumn<bool>(
                name: "ShowTypingText",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "ShowTypingText",
                value: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "ShowTypingText",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowTypingText",
                table: "Users");
        }
    }
}
