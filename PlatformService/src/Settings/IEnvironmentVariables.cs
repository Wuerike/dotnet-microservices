using PlatformService.Settings.Models;

namespace PlatformService.Settings
{
    public interface IEnvironmentVariables
    {
        CommandApiVariables CommandApiVariables();
        DatabaseVariables DatabaseVariables();
        MessageBusVariables MessageBusVariables();
    }
}