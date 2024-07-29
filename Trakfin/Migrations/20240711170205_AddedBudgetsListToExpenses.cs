using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trakfin.Migrations
{
    /// <inheritdoc />
    public partial class AddedBudgetsListToExpenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpenseId",
                table: "Budget",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Budget_ExpenseId",
                table: "Budget",
                column: "ExpenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Expense_ExpenseId",
                table: "Budget",
                column: "ExpenseId",
                principalTable: "Expense",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Expense_ExpenseId",
                table: "Budget");

            migrationBuilder.DropIndex(
                name: "IX_Budget_ExpenseId",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "ExpenseId",
                table: "Budget");
        }
    }
}
