using CommandService.DataServices.Sync.Grpc;
using CommandService.Models;

namespace CommandService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using ( var serviceScope = app.ApplicationServices.CreateScope())
            {
                var repo = serviceScope.ServiceProvider.GetService<ICommandRepo>();
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                var platforms = grpcClient.GetAllPlatforms();

                SeedData(platforms, repo);    
            }
        }

        public static void SeedData(IEnumerable<Platform> platforms, ICommandRepo repo)
        {
            Console.WriteLine($"--> Seeding platforms incomming from PlatformAPI");
            
            foreach (var p in platforms)
            {
                if(!repo.PlatformExists(p.Id))
                {
                    repo.CreatePlatform(p);
                    repo.SaveChanges();
                }

            }
        }
    }
}