using CommandService.DataServices.Sync.Grpc;
using CommandService.Models;
using CommandService.Settings;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using ( var serviceScope = app.ApplicationServices.CreateScope())
            {
                var repo = serviceScope.ServiceProvider.GetService<ICommandRepo>();
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                var variables = serviceScope.ServiceProvider.GetService<IEnvironmentVariables>();
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                var platforms = grpcClient.GetAllPlatforms();

                if(!bool.Parse(variables.DatabaseVariables().InMemoryDb))
                {
                    Log.Information("Trying to apply migrations");
                    try
                    {
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Could not migrate: {ex.Message}");
                    }
                }

                SeedData(repo, platforms);    
            }
        }

        public static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
        {
            try
            {
                foreach (var p in platforms)
                {
                    if(!repo.PlatformExists(p.Id))
                    {
                        repo.CreatePlatform(p);
                        repo.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error($"Could not seed data comming from Platform Api by GRPC: {ex.Message}");
            }
        }
    }
}