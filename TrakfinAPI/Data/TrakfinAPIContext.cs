using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TrakfinAPI.Models;

namespace TrakfinAPI.Data
{
    public class TrakfinAPIContext : DbContext
    {
        public TrakfinAPIContext(DbContextOptions<TrakfinAPIContext> options)
            : base(options)
        {
            var dbCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (dbCreator != null)
            {
                // Create Database 
                if (!dbCreator.CanConnect())
                {
                    dbCreator.Create();
                }

                // Create Tables
                if (!dbCreator.HasTables())
                {
                    dbCreator.CreateTables();
                }
            }
        }

        public DbSet<Expense> Expense { get; set; } = default!;
        public DbSet<Budget> Budget { get; set; } = default!;
        public DbSet<CustomFilter> CustomFilter { get; set; } = default!;
        public DbSet<Subscription> Subscription { get; set; } = default!;
    }
}
