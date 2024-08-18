using Microsoft.EntityFrameworkCore;
using TrakfinAPI.Models;

namespace TrakfinAPI.Data
{
    public class TrakfinAPIContext : DbContext
    {
        public TrakfinAPIContext(DbContextOptions<TrakfinAPIContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expense { get; set; } = default!;
        public DbSet<Budget> Budget { get; set; } = default!;
        public DbSet<CustomFilter> CustomFilter { get; set; } = default!;
        public DbSet<Subscription> Subscription { get; set; } = default!;
    }
}
