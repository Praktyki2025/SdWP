using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SdWP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CostCategory> CostCategories { get; set; }
        public DbSet<CostType> CostTypes { get; set; }
        public DbSet<ErrorLog> ErrorLog { get; set; }
        public DbSet<Link> Link { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<UserGroupType> UserGroupTypes { get; set; }
        public DbSet<Valuation> Valuation { get; set; }
        public DbSet<ValuationItem> ValuationItems { get; set; }

        public DbSet<User>users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Valuation>()
                .HasOne(v => v.Projects)
                .WithMany(p => p.Valuations)
                .HasForeignKey(v => v.ProjectId);

            builder.Entity<Valuation>()
                .HasOne(v => v.CreatorUser)
                .WithMany(u => u.Valuations)
                .HasForeignKey(v => v.CreatorUserId);

            builder.Entity<ValuationItem>()
                .HasOne(vi => vi.Valuation)
                .WithMany(v => v.ValuationItems)
                .HasForeignKey(vi => vi.ValuationId);

            builder.Entity<Link>()
                .HasOne(l => l.Projects)
                .WithMany(p => p.Links)
                .HasForeignKey(l => l.ProjectId);

            builder.Entity<Link>()
                .HasOne(l => l.Valuation)
                .WithMany(v => v.Links)
                .HasForeignKey(l => l.ValuationId);

            builder.Entity<CostType>()
                .HasMany(ct => ct.ValuationItems) 
                .WithOne(vi => vi.CostType) 
                .HasForeignKey(vi => vi.CostTypeId); 

            builder.Entity<CostCategory>()
                .HasMany(c => c.ValuationItems) 
                .WithOne(vi => vi.CostCategory) 
                .HasForeignKey(vi => vi.CostCategoryID); 

            builder.Entity<UserGroupType>()
                .HasMany(u => u.ValuationItems) 
                .WithOne(vi => vi.UserGroupType) 
                .HasForeignKey(vi => vi.UserGroupTypeId);

            builder.Entity<ErrorLog>()
                .HasOne(e => e.User)
                .WithMany(u => u.ErrorLog)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(u => u.Name);

            builder.Entity<User>()
                .HasMany(p => p.Projects) 
                .WithMany(u => u.User) 
                .UsingEntity(j => j.ToTable("ProjectUsers")); 
        }
    }
}
