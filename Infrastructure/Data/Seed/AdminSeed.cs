using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Seed;

public static class AdminSeed
{
    public static async Task SeedAdminsAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
        }

        var adminUsers = new[]
        {
            new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@talentplus.com",
                EmailConfirmed = true
            }
        };

        foreach (var admin in adminUsers)
        {
            var existingUser = await userManager.FindByEmailAsync(admin.Email);
            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(admin, "Admin123$");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(existingUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(existingUser, "Admin");
                }
            }
        }
    }
}