using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task SeedDatabase(this WebApplication webApplication)
        {
            using var scope = webApplication.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync();
                await Seed.SeedData(context);
            }
            catch (Exception ex) 
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration");
            }
        }
    }
}
