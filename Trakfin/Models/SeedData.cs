using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.EntityFrameworkCore;
using Trakfin.Data;
using Trakfin.Migrations;
using static Azure.Core.HttpHeader;

namespace Trakfin.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new TrakfinContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<TrakfinContext>>()))
            {
                if (context.Expense.Any())  
                {
                    return; // DB has been seeded
                }
                context.Expense.AddRange(
                    new Expense
                    {
                        Title = "Vet visit with my dog",
                        Date = DateTime.Parse("20.06.2024"),
                        Bank = "Bank of Luxemburg",
                        Price = 250.00M,
                        Category = "Animals",
                        Recurring = ExpenseRecurring.Yes,
                        Tags = "Vet, Animal Health",
                        Status = ExpenseStatus.Pending,
                    },
                    new Expense
                    {
                        Title = "Grocery shopping",
                        Date = DateTime.Parse("22.06.2024"),
                        Bank = "Santander Bank",
                        Price = 300.25M,
                        Note = "Also I bought snacks for party",
                        Category = "Shopping",
                        PaymentMethod = ExpensePaymentMethod.ApplePay,
                        Recurring = ExpenseRecurring.No,
                        MerchantOrVendor = "Walmart",
                        Tags = "Party, Grocery",
                        Status = ExpenseStatus.Paid,
                    },
                    new Expense
                    {
                        Title = "Car rental",
                        Date = DateTime.Parse("24.06.2024"),
                        Bank = "Bank of Ireland",
                        Price = 1500.00M,
                        Note = "BMW M4 Competition for 3 days",
                        Category = "Cars",
                        PaymentMethod = ExpensePaymentMethod.CreditCard,
                        MerchantOrVendor = "Warsaw Rental Centre",
                        Tags = "Cars",
                        Status = ExpenseStatus.NotPaid,
                    }
                );

                if (context.Subscription.Any())
                {
                    return; // DB has been seeded
                }

                context.Subscription.AddRange(
                    new Subscription
                    {
                        Name = "Netflix",
                        Category = "Streaming",
                        Price = 15.99M,
                        BillingCycle = SubscriptionBillingCycle.Monthly,
                        StartDate = DateTime.Parse("30.06.2024"),
                        NextBillingDate = DateTime.Parse("30.07.2024"),
                        Status = SubscriptionStatus.Active,
                        PaymentMethod = SubscriptionPaymentMethod.PayPal,
                        CancellationPolicy = SubscriptionCancellationPolicy.WebsitePortal,
                        Notes = "Family Subscription",
                        Provider = "Netflix, Inc.",
                        User = "test_user",
                        Duration = "6 months",
                        Discount = "Student discount -10%"
                    },
                    new Subscription
                    {
                        Name = "Adobe Photoshop",
                        Category = "Software",
                        Price = 20.99M,
                        BillingCycle = SubscriptionBillingCycle.Monthly,
                        StartDate = DateTime.Parse("1.07.2024"),
                        NextBillingDate = DateTime.Parse("1.08.2024"),
                        Status = SubscriptionStatus.Active,
                        PaymentMethod = SubscriptionPaymentMethod.CreditCard,
                        CancellationPolicy = SubscriptionCancellationPolicy.FeedbackAndCancellationSurvey,
                        Notes = "For professional use",
                        Provider = "Adobe",
                        User = "hellouser",
                        Duration = "1 year",
                        Discount = "None"
                    },
                    new Subscription
                    {
                        Name = "NY Times",
                        Category = "Software",
                        Price = 5.00M,
                        BillingCycle = SubscriptionBillingCycle.Monthly,
                        StartDate = DateTime.Parse("2.07.2024"),
                        NextBillingDate = DateTime.Parse("2.08.2024"),
                        Status = SubscriptionStatus.Active,
                        PaymentMethod = SubscriptionPaymentMethod.CreditCard,
                        CancellationPolicy = SubscriptionCancellationPolicy.InApp,
                        Notes = "Digital access only",
                        Provider = "NY Times",
                        User = "john_doe11",
                        Duration = "1 month",
                        Discount = "Introductory offer"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
