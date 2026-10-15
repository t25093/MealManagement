using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class QuantitiesToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Dishes_DishId1",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_DishId1",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "DishId1",
                table: "Recipes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DishId1",
                table: "Recipes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_DishId1",
                table: "Recipes",
                column: "DishId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Dishes_DishId1",
                table: "Recipes",
                column: "DishId1",
                principalTable: "Dishes",
                principalColumn: "Id");
        }
    }
}
