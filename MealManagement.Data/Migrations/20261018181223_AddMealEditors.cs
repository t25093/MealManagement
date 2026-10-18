using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MealManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMealEditors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealEditors",
                columns: table => new
                {
                    EditableMealsId = table.Column<int>(type: "integer", nullable: false),
                    EditorsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealEditors", x => new { x.EditableMealsId, x.EditorsId });
                    table.ForeignKey(
                        name: "FK_MealEditors_Meals_EditableMealsId",
                        column: x => x.EditableMealsId,
                        principalTable: "Meals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealEditors_Users_EditorsId",
                        column: x => x.EditorsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealEditors_EditorsId",
                table: "MealEditors",
                column: "EditorsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealEditors");
        }
    }
}
