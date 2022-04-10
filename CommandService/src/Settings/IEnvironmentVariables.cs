namespace CommandService.Settings
{
    public interface IEnvironmentVariables
    {
        IDictionary<string, string> PlatformApi();
        IDictionary<string, string> MessageBus();
    }
}