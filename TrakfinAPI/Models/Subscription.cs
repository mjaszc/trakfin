using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrakfinAPI.Models
{
    public enum SubscriptionStatus
    {
        Active,
        Paused,
        Canelled
    }

    public enum SubscriptionPaymentMethod
    {
        [Display(Name = "Credit Card")]
        CreditCard,

        [Display(Name = "Bank Transfer")]
        BankTransfer,

        [Display(Name = "Apple Pay")]
        ApplePay,

        PayPal
    }

    public enum SubscriptionBillingCycle
    {
        None = 0,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    public enum SubscriptionCancellationPolicy
    {
        [Display(Name = "In App Cancellation")]
        InApp,

        [Display(Name = "Website Portal")]
        WebsitePortal,

        [Display(Name = "Customer Support Request")]
        CustomerSupportRequest,

        [Display(Name = "App Store Cancellation")]
        AppStore,

        [Display(Name = "Automatic Renewal Disable")]
        AutomaticRenewal,

        [Display(Name = "Grace Period and Refund Policy")]
        GracePeriodAndRefundPolicy,

        [Display(Name = "Feedback and Cancellation Survey")]
        FeedbackAndCancellationSurvey,
    }

    public class Subscription
    {
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3), Required]
        public string? Name { get; set; }

        [StringLength(100, MinimumLength = 3)]
        public string? Category { get; set; }

        [DataType(DataType.Currency), Column(TypeName = "decimal(18, 2)"), Required]
        public decimal Price { get; set; }

        [Display(Name = "Billing Cycle")]
        public SubscriptionBillingCycle? BillingCycle { get; set; } = null;

        [Display(Name = "Start Date"), DataType(DataType.Date), Required]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Next Billing Date"), DataType(DataType.Date), Required]
        public DateTime? NextBillingDate { get; set; }

        public SubscriptionStatus? Status { get; set; } = null;

        [Display(Name = "Payment Method")]
        public SubscriptionPaymentMethod? PaymentMethod { get; set; } = null;

        // public string? Notifications // ADD THAT COLUMN AFTER IMPLEMENTING NOTIFICATIONS

        [Display(Name = "Cancellation Policy")]
        public SubscriptionCancellationPolicy? CancellationPolicy { get; set; } = null;

        [StringLength(250, MinimumLength = 3)]
        public string? Notes { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public string? Provider { get; set; }

        [StringLength(20, MinimumLength = 3)]
        public string? User { get; set; }

        [StringLength(15, MinimumLength = 3)]
        public string? Duration { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public string? Discount { get; set; }
    }
}
