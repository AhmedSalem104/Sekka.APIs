using Microsoft.Extensions.DependencyInjection;

namespace Sekka.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<SekkaDbContext>();
        await context.Database.EnsureCreatedAsync();

        // TODO: Add seed data from JSON files (regions.json, vehicleTypes.json, etc.)
    }
}
