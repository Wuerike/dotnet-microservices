namespace CommandService.Settings
{
    public interface IEnvironmentVariables
    {
        IDictionary<string, string> PlatformApiVariables();
        IDictionary<string, string> MessageBusVariables();
    }
}