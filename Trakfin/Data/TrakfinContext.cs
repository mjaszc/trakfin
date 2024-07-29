using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public DbSet<Trakfin.Models.Expense> Expense { get; set; }
        public DbSet<Trakfin.Models.Subscription> Subscription { get; set; }
        public DbSet<Trakfin.Models.Budget> Budget { get; set; }
        public DbSet<Trakfin.Models.CustomFilter> CustomFilter { get; set; }
    }
}
