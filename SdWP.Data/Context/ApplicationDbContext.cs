using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SdWP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

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
        public DbSet<ValuationItem> ValuationItem { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Projects>()
             .HasOne(p => p.CreatorUser)
             .WithMany(u => u.CreatedProjects)
             .HasForeignKey(p => p.CreatorUserId);

            builder.Entity<Valuation>()
                .HasOne(v => v.Projects)
                .WithMany(p => p.Valuations)
                .HasForeignKey(v => v.ProjectId);

            builder.Entity<Valuation>()
                .HasOne(v => v.CreatorUser)
                .WithMany(u => u.Valuation)
                .HasForeignKey(v => v.CreatorUserId);

            builder.Entity<ValuationItem>()
                .HasOne(vi => vi.Valuation)
                .WithMany(v => v.ValuationItems)
                .HasForeignKey(vi => vi.ValuationId);

            builder.Entity<ValuationItem>()
                .HasOne(vi => vi.CostType)
                .WithMany(ct => ct.ValuationItem)
                .HasForeignKey(vi => vi.CostTypeId);

            builder.Entity<ValuationItem>()
                .HasOne(vi => vi.CostCategory)
                .WithMany(cc => cc.ValuationItem)
                .HasForeignKey(vi => vi.CostCategoryId);

            builder.Entity<ValuationItem>()
                .HasOne(vi => vi.UserGroupType)
                .WithMany(ugt => ugt.ValuationItem)
                .HasForeignKey(vi => vi.UserGroupTypeId);

            builder.Entity<ErrorLog>()
                .HasOne(el => el.User)
                .WithMany(u => u.ErrorLogs)
                .HasForeignKey(el => el.UserId);

        }
    }
}
