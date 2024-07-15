using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trakfin.Migrations
{
    /// <inheritdoc />
    public partial class OneToManyBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Expense_ExpenseId",
                table: "Budget");

            migrationBuilder.RenameColumn(
                name: "ExpenseId",
                table: "Budget",
                newName: "BudgetId");

            migrationBuilder.RenameIndex(
                name: "IX_Budget_ExpenseId",
                table: "Budget",
                newName: "IX_Budget_BudgetId");

            migrationBuilder.AddColumn<int>(
                name: "BudgetId",
                table: "Expense",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expense_BudgetId",
                table: "Expense",
                column: "BudgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Budget_BudgetId",
                table: "Budget",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_Budget_BudgetId",
                table: "Expense",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_Budget_BudgetId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_Expense_Budget_BudgetId",
                table: "Expense");

            migrationBuilder.DropIndex(
                name: "IX_Expense_BudgetId",
                table: "Expense");

            migrationBuilder.DropColumn(
                name: "BudgetId",
                table: "Expense");

            migrationBuilder.RenameColumn(
                name: "BudgetId",
                table: "Budget",
                newName: "ExpenseId");

            migrationBuilder.RenameIndex(
                name: "IX_Budget_BudgetId",
                table: "Budget",
                newName: "IX_Budget_ExpenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_Expense_ExpenseId",
                table: "Budget",
                column: "ExpenseId",
                principalTable: "Expense",
                principalColumn: "Id");
        }
    }
}
