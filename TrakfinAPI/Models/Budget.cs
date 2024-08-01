using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrakfinAPI.Models
{
    public enum BudgetStatus
    {
        Active,
        Inactive
    }

    public class Budget
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3), Required]
        public string? Name { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public string? Category { get; set; }

        [DataType(DataType.Currency), Column(TypeName = "decimal(18, 2)"), Required]
        [Range(10, double.MaxValue, ErrorMessage = "The budget amount must be at least 10")]
        public decimal? BudgetAmount { get; set; }

        [DataType(DataType.Currency), Column(TypeName = "decimal(18, 2)"), Required]
        public decimal? SpentAmount { get; set; }

        [DataType(DataType.Date), Required]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date), Required]
        public DateTime? EndDate { get; set; }

        public BudgetStatus? Status { get; set; } = null;

        [StringLength(250, MinimumLength = 3)]
        public string? Notes { get; set; }

        [StringLength(60, MinimumLength = 3)]
        public string? Tags { get; set; }

        public ICollection<Expense> Expenses { get; } = new List<Expense>();
    }
}
