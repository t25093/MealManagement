using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserDishAssignments",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "PendingEmail",
                table: "UserDishAssignments",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDishAssignments_PendingEmail",
                table: "UserDishAssignments",
                column: "PendingEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserDishAssignments_PendingEmail",
                table: "UserDishAssignments");

            migrationBuilder.DropColumn(
                name: "PendingEmail",
                table: "UserDishAssignments");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserDishAssignments",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
