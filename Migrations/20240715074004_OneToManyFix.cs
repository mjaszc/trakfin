using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trakfin.Migrations
{
    /// <inheritdoc />
    public partial class OneToManyFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Budget_BudgetId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Budget_BudgetId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "Budget");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetId",
                table: "Budget",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Budget_BudgetId",
                table: "Budget",
                column: "BudgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Budget_BudgetId",
                table: "Budget",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id");
        }
    }
}
