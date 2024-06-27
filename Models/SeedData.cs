using Microsoft.EntityFrameworkCore;
using Trakfin.Data;

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
                // Look for any expenses
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
                    },
                    new Expense
                    {
                        Title = "Grocery shopping",
                        Date = DateTime.Parse("22.06.2024"),
                        Bank = "Santander Bank",
                        Price = 300.25M,
                        Note = "Also I bought snacks for party"
                    },
                    new Expense
                    {
                        Title = "Car rental",
                        Date = DateTime.Parse("24.06.2024"),
                        Bank = "Bank of Ireland",
                        Price = 1500.00M,
                        Note = "BMW M4 Competition for 3 days"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
