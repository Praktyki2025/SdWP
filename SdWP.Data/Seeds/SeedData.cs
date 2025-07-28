using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SdWP.Data.Models;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
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
    }
}
