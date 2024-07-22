using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Trakfin.Models
{
    public class ExpenseViewModel
    {
        public List<Expense>? Expenses { get; set; }
        public List<Expense>? RecurringTransactions { get; set; }
        public List<CustomFilter>? CustomFilters { get; set; }

        public SelectList? Banks { get; set; }
        public SelectList? Categories { get; set; }

        public string? SearchString { get; set; }
        public string? BankName { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Dictionary<int, string>? BudgetNames { get; set; }
    }
        
}
