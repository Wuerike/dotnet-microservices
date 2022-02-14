using Microsoft.EntityFrameworkCore;
using PlatformApi.Models;

namespace PlatformApi.Data
{
    public class AppDbContext :  DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
            
        }

        public DbSet<Platform> Platforms { get; set; }
    }    
}