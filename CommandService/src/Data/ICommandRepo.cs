using System.ComponentModel;
using CommandService.Models;

namespace CommandService.Data
{
    interface ICommandRepo
    {
        bool SaveChanges();

        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform p);
        bool PlatformExists(int platformId);

        IEnumerable<Command> GetCommandsByPlatformId(int platformId);
        Command GetCommand(int commandId, int platformId);
        void CreateCommand(int platformId, Command c);
    }
}