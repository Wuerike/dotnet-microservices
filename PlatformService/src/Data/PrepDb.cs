using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool IsProduction)
        {
            using ( var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), IsProduction);    
            }
        }

        public static void SeedData(AppDbContext context, bool IsProduction)
        {
            if(IsProduction)
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