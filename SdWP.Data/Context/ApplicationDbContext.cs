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
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CostCategory> CostCategories { get; set; }
        public DbSet<CostType> CostTypes { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<UserGroupType> UserGroupTypes { get; set; }
        public DbSet<Valuation> Valuations { get; set; }
        public DbSet<ValuationItem> ValuationItems { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
                

            builder.Entity<Valuation>()
                .HasOne(v => v.Project)
                .WithMany(p => p.Valuations)
                .HasForeignKey(v => v.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Valuation>()
                .HasOne(v => v.CreatorUser)
                .WithMany(u => u.Valuations)
                .HasForeignKey(v => v.CreatorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ValuationItem>()
                .HasOne(vi => vi.Valuation)
                .WithMany(v => v.ValuationItems)
                .HasForeignKey(vi => vi.ValuationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ValuationItem>()
                .HasOne(p => p.CreatorUser);


            builder.Entity<Link>()
                .HasOne(l => l.Project)
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
                .WithMany(u => u.ErrorLogs)
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(u => u.Id);

            builder.Entity<Project>()
                .HasMany(p => p.Users)
                .WithMany(u => u.Projects)
                .UsingEntity<Dictionary<string, object>>(
        "XProjectUsers",
        x => x
            .HasOne<User>()
            .WithMany()
            .HasForeignKey("UsersId")
            .OnDelete(DeleteBehavior.Cascade),
        x => x
            .HasOne<Project>()
            .WithMany()
            .HasForeignKey("ProjectsId")
            .OnDelete(DeleteBehavior.Restrict)
    );

            builder.Entity<Project>()
                .HasOne(p => p.CreatorUser);

            builder.Entity<ValuationItem>()
                .Property(vi => vi.TotalAmount)
                .HasPrecision(18, 2);

            builder.Entity<ValuationItem>()
                .Property(vi => vi.Quantity)
                .HasPrecision(18, 2);


            builder.Entity<User>()
                .Property(u => u.Name)
                .HasMaxLength(256);
        }
    }
}
