namespace PlatformService.Settings
{
    public interface IEnvironmentVariables
    {
        IDictionary<string, string> ApplicationVariables();
        IDictionary<string, string> CommandApiVariables();
        IDictionary<string, string> DatabaseVariables();
        IDictionary<string, string> MessageBusVariables();
    }
}