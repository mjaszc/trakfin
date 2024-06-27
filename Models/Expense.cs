using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Trakfin.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3), Required]
        public string? Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [StringLength(60, MinimumLength = 3), RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")]
        public string? Bank { get; set; }

        [DataType(DataType.Currency), Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public string? Note { get; set; }
    }
}
