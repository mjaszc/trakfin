using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Trakfin.Models
{
    public class BankNameViewModel
    {
        public List<Expense>? Expenses { get; set; }
        public SelectList? Banks { get; set; }

        public string? BankName { get; set; }
        public string? SearchString { get; set; }
    }
        
}
