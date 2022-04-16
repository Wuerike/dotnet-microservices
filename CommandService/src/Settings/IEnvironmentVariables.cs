using CommandService.Settings.Models;

namespace CommandService.Settings
{
    public interface IEnvironmentVariables
    {
        PlatformApiVariables PlatformApiVariables();
        MessageBusVariables MessageBusVariables();
    }
}