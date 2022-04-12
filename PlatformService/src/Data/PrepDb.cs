using Microsoft.EntityFrameworkCore;
using PlatformService.Models;
using PlatformService.Settings;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using ( var serviceScope = app.ApplicationServices.CreateScope())
            {
                var variables = serviceScope.ServiceProvider.GetService<IEnvironmentVariables>();
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), variables);    
            }
        }

        public static void SeedData(AppDbContext context, IEnvironmentVariables variables)
        {
            var inMemoryDb = bool.Parse(variables.ApplicationVariables()["InMemoryDb"]);

            if(!inMemoryDb)
            {
                Console.WriteLine("--> Trying to apply migrations");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not migrate: {ex.Message}");
                }
            }

            if(!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding data");

                context.Platforms.AddRange(
                    new Platform(){Name="Dotnet", Publisher="Microsoft", Cost="Free"},
                    new Platform(){Name="SQL Server Express", Publisher="Microsoft", Cost="Free"},
                    new Platform(){Name="Kubernetes", Publisher="Cloud Native Computing Foundation", Cost="Free"}
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> Data already seeded");
            }
        }
    }
}