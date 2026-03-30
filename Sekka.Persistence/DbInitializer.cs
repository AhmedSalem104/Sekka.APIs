using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Sekka.Core.Common;
using Sekka.Persistence.Entities;

namespace Sekka.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<SekkaDbContext>();
        await context.Database.EnsureCreatedAsync();

        // Seed Roles
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        string[] roles = ["Driver", "Admin", "Support"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
        }

        // Seed Admin User: Ahmed Salem (01015819700)
        var userManager = serviceProvider.GetRequiredService<UserManager<Driver>>();
        var adminPhone = EgyptianPhoneHelper.Normalize("01015819700");
        var adminUser = await userManager.FindByNameAsync(adminPhone);
        if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
