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

        public DbSet<Expense> Expense { get; set; } = null!;
        public DbSet<Budget> Budget { get; set; } = default!;
    }
}
