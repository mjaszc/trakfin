using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trakfin.Models;

namespace Trakfin.Data
{
    public class TrakfinContext : DbContext
    {
        public TrakfinContext (DbContextOptions<TrakfinContext> options)
            : base(options)
        {
        }

        public DbSet<Trakfin.Models.Expense> Expense { get; set; } = default!;
        public DbSet<Trakfin.Models.Subscription> Subscription { get; set; }
    }
}
