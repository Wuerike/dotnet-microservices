namespace PlatformService.Settings
{
    public interface IEnvironmentVariables
    {
        string GetConnectionString();

        string GetCommandServiceUrl();

        IDictionary<string, string> MessageBusVariables();

        bool IsInMemoryDb();
    }
}