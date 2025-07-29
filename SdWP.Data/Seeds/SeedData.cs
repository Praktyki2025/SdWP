using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SdWP.Data.Context;
using SdWP.Data.Models;
using System.Security.Cryptography;

public static class SeedData
{
    public static async Task SeedProjects(IServiceProvider serviceProvider, User user)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        var projectCount = await context.Projects.CountAsync(p => p.CreatorUserId == user.Id);

        if (projectCount < 3)
        {
            for (int i = 0; i < 3 - projectCount; i++)
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
                LastUpdate = DateTime.UtcNow
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
                LastUpdate = DateTime.UtcNow
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

    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        await SeedUsers(serviceProvider);
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
}
