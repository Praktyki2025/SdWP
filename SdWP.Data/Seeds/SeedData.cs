using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SdWP.Data.Models;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = "admin@example.pl";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Admin User",
            };
            var createAdminResult = await userManager.CreateAsync(adminUser, "Admin123!");
            if (createAdminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        var userEmail = "user@example.pl";
        var regularUser = await userManager.FindByEmailAsync(userEmail);

        if (regularUser == null)
        {
            regularUser = new User
            {
                UserName = userEmail,
                Email = userEmail,
                Name = "Regular User",
            };
            var createUserResult = await userManager.CreateAsync(regularUser, "User123!");
            if (createUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(regularUser, "User");
            }
        }
    }
}