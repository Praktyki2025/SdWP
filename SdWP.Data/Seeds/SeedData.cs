using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SdWP.Data.Context;
using SdWP.Data.Models;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        await SeedUsers(serviceProvider);

        await SeedCostTypes(serviceProvider);
        await SeedCostCategories(serviceProvider);

        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var adminUser = await userManager.FindByEmailAsync("admin@example.pl");
        if (adminUser != null)
        {
            await SeedProjects(serviceProvider, adminUser);
        }

        var normalUser = await userManager.FindByEmailAsync("user@example.pl");
        if (normalUser != null)
        {
            await SeedProjects(serviceProvider, normalUser);
        }
    }

    public static async Task SeedProjects(IServiceProvider serviceProvider, User user)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var projectCount = await context.Projects.CountAsync(p => p.CreatorUserId == user.Id);

        if (projectCount < 20)
        {
            for (int i = 0; i < 20 - projectCount; i++)
            {
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    Title = $"Title{i}",
                    Description = "Description",
                    CreatedAt = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    CreatorUserId = user.Id
                };

                await context.Projects.AddAsync(project);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var newRole = new IdentityRole<Guid> { Name = role, NormalizedName = role.ToUpper() };
                await roleManager.CreateAsync(newRole);
            }
        }

        var adminEmail = "admin@example.pl";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new User
            {
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToUpper(),
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Name = "Administrator",
                CreatedAt = DateTime.UtcNow,
                LastUpdate = DateTime.UtcNow,
                EmailConfirmed = true,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        Console.WriteLine($"Admin - {adminUser.Id}");

        var userEmail = "user@example.pl";
        var normalUser = await userManager.FindByEmailAsync(userEmail);
        if (normalUser == null)
        {
            normalUser = new User
            {
                Email = userEmail,
                NormalizedEmail = userEmail.ToUpper(),
                UserName = "User",
                NormalizedUserName = "USER",
                Name = "User",
                CreatedAt = DateTime.UtcNow,
                LastUpdate = DateTime.UtcNow,
                EmailConfirmed = true,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(normalUser, "User123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(normalUser, "User");
            }
        }
        Console.WriteLine($"Admin - {adminUser.Id}");
        Console.WriteLine($"User - {normalUser.Id}");
    }

    public static async Task SeedCostTypes(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        if (!await context.CostTypes.AnyAsync())
        {
            var costTypes = new List<CostType>
            {
                new CostType { Id = Guid.NewGuid(), Name = "Type1" },
                new CostType { Id = Guid.NewGuid(), Name = "Type2" },
                new CostType { Id = Guid.NewGuid(), Name = "Type3" }
            };
            await context.CostTypes.AddRangeAsync(costTypes);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedCostCategories(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        if (!await context.CostCategories.AnyAsync())
        {
            var costCategories = new List<CostCategory>
            {
                new CostCategory { Id = Guid.NewGuid(), Name = "Category1" },
                new CostCategory { Id = Guid.NewGuid(), Name = "Category2" },
                new CostCategory { Id = Guid.NewGuid(), Name = "Category3" }
            };
            await context.CostCategories.AddRangeAsync(costCategories);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedValuation(IServiceProvider serviceProvider, User user)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var projects = await context.Projects.Where(p => p.CreatorUserId == user.Id).ToListAsync();
        var costTypes = await context.CostTypes.FirstOrDefaultAsync();
        var costCategories = await context.CostCategories.FirstOrDefaultAsync();
        var userGroupTypes = await context.UserGroupTypes.FirstOrDefaultAsync();

        foreach (var project in projects)
        {
            if (!context.Valuations.Any(v => v.ProjectId == project.Id))
            {
                var valuation = new Valuation
                {
                    Id = Guid.NewGuid(),
                    Name = $"Valuation for {project.Title}",
                    Description = "Sample valuation description",
                    ProjectId = project.Id,
                    CreatorUserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

                await context.Valuations.AddAsync(valuation);
                await context.SaveChangesAsync();

                var valutaionItem = new List<ValuationItem>
                {
                    new ValuationItem
                    {
                        Id = Guid.NewGuid(),
                        ValuationId = valuation.Id,
                        Name = "Labor Cost Item",
                        Description = "Labor cost description",
                        CreatorUserId = user.Id,
                        CreatedAt = DateTime.UtcNow,
                        LastModified = DateTime.UtcNow,
                        CostTypeId = costTypes?.Id ?? Guid.Empty,
                        CostCategoryID = costCategories?.Id ?? Guid.Empty,
                        UserGroupTypeId = userGroupTypes?.Id ?? Guid.Empty,
                        Quantity = 10,
                        UnitPrice = 20,
                        TotalAmount = 200,
                        RecurrencePeriod = 0,
                        RecurrenceUnit = null
                    },

                    new ValuationItem
                    {
                        Id = Guid.NewGuid(),
                        ValuationId = valuation.Id,
                        Name = "Material Cost Item",
                        Description = "Material cost description",
                        CreatorUserId = user.Id,
                        CreatedAt = DateTime.UtcNow,
                        LastModified = DateTime.UtcNow,
                        CostTypeId = costTypes?.Id ?? Guid.Empty,
                        CostCategoryID = costCategories?.Id ?? Guid.Empty,
                        UserGroupTypeId = userGroupTypes?.Id ?? Guid.Empty,
                        Quantity = 5,
                        UnitPrice = 50,
                        TotalAmount = 250,
                        RecurrencePeriod = 1,
                        RecurrenceUnit = "Month"
                    }
                };

                await context.ValuationItems.AddRangeAsync(valutaionItem);
                await context.SaveChangesAsync();
            }
        }
    }
}
