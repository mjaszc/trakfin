using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Trakfin.Models;

namespace Trakfin.Data
{
    public class TrakfinContext : IdentityDbContext<IdentityUser>
    {
        public TrakfinContext(DbContextOptions<TrakfinContext> options) : base(options)
        {
        }

        public DbSet<Expense> Expense { get; set; }
        public DbSet<Subscription> Subscription { get; set; }
        public DbSet<Budget> Budget { get; set; }
        public DbSet<CustomFilter> CustomFilter { get; set; }
    }
}
