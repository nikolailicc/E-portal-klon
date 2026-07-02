using Eportal.Modules.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Eportal.Modules.Identity.Infrastructure;

public static class AdminSeeder
{
    private const string AdminEmail = "admin@eportal.local";
    private const string AdminPassword = "Admin123!";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        if (await userManager.FindByEmailAsync(AdminEmail) is not null)
        {
            return;
        }

        var admin = new AppUser
        {
            UserName = AdminEmail,
            Email = AdminEmail,
            FirstName = "Sistem",
            LastName = "Administrator",
            Role = UserRole.Administrator,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, AdminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, UserRole.Administrator.ToString());
        }
    }
}