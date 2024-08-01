using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrakfinAPI.Models
{
    public enum ExpensePaymentMethod
    {
        [Display(Name = "Credit Card")]
        CreditCard,

        [Display(Name = "Bank Transfer")]
        BankTransfer,

        [Display(Name = "Apple Pay")]
        ApplePay,

        PayPal
    }

    public enum ExpenseRecurring
    {
        Yes,
        No
    }

    public enum ExpenseStatus
    {
        Pending,
        Paid,

        [Display(Name = "Not Paid")]
        NotPaid
    }

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
        [Range(0.1, double.MaxValue, ErrorMessage = "The price must be at least 0.1")]
        public decimal Price { get; set; }

        [StringLength(60, MinimumLength = 3)]
        public string? Category { get; set; }

        [StringLength(250, MinimumLength = 3)]
        public string? Note { get; set; }

        [Display(Name = "Payment Method")]
        public ExpensePaymentMethod? PaymentMethod { get; set; } = null;

        public ExpenseRecurring? Recurring { get; set; } = null;

        [Display(Name = "Merchant / Vendor")]
        public string? MerchantOrVendor { get; set; }

        [StringLength(60, MinimumLength = 3)]
        public string? Tags { get; set; }

        public ExpenseStatus? Status { get; set; } = null;

        public int? BudgetId { get; set; }

        public Budget? Budget { get; set; }
    }
}
