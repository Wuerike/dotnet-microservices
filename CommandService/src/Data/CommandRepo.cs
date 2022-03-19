using System.Linq;
using CommandService.Models;

namespace CommandService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;

        public CommandRepo(AppDbContext context)
        {
            _context = context;
        }

        public void CreateCommand(int platformId, Command c)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }

            c.PlatformId = platformId;
            _context.Commands.Add(c);
        }

        public void CreatePlatform(Platform p)
        {
            if (p == null)
            {
                throw new ArgumentNullException(nameof(p));
            }

            _context.Platforms.Add(p);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Command GetCommand(int commandId, int platformId)
        {
            return _context.Commands
                .Where(c => c.Id == commandId && c.PlatformId == platformId).FirstOrDefault();
        }

        public IEnumerable<Command> GetCommandsByPlatformId(int platformId)
        {
            return _context.Commands
                .Where(c => c.PlatformId == platformId)
                .OrderBy(c => c.Platform.Name);
        }

        public bool PlatformExists(int platformId)
        {
            return _context.Platforms.Any(p => p.Id == platformId);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}