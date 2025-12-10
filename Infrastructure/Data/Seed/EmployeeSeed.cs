using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Seed;

public static class EmployeeSeed
{
    public static async Task SeedEmployeeAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Employee"))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = "Employee" });
        }

        var employeeUsers = new[]
        {
            new ApplicationUser
            {
                UserName = "employee",
                Email = "employee@talentplus.com",
                EmailConfirmed = true
            }
        };

        foreach (var employee in employeeUsers)
        {
            var existingUser = await userManager.FindByEmailAsync(employee.Email);
            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(employee, "123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employee, "Employee");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(existingUser, "Employee"))
                {
                    await userManager.AddToRoleAsync(existingUser, "Employee");
                }
            }
        }
    }
}