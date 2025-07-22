using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SdWP.Data.Models;

namespace SdWP.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<CostCategory> CostCategories { get; set; }
        public DbSet<CostType> CostTypes { get; set; }
        public DbSet<ErrorLog> ErrorLog{ get; set; }
        public DbSet<Link> Link { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<UserGroupType> UserGroupTypes { get; set; }
        public DbSet<Valuation> Valuation { get; set; }
        public DbSet<ValuationItem> ValuationItems { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
