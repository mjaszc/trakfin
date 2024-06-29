using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Trakfin.Models
{
    public class ExpenseViewModel
    {
        public List<Expense>? Expenses { get; set; }

        public SelectList? Banks { get; set; }

        public SelectList? Categories { get; set; }

        public List<Expense>? Dates { get; set; }

        public string? SearchString { get; set; }

        public string? BankName { get; set; }

        public string? CategoryName { get; set; }
        
        public DateTime? Date { get; set; }
        
    }
        
}
