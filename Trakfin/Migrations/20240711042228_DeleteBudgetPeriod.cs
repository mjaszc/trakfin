using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trakfin.Migrations
{
    /// <inheritdoc />
    public partial class DeleteBudgetPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BudgetPeriod",
                table: "Budget");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BudgetPeriod",
                table: "Budget",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);
        }
    }
}
