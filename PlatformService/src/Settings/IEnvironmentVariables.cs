namespace PlatformService.Settings
{
    public interface IEnvironmentVariables
    {
        string GetConnectionString();

        string GetCommandServiceUrl();
    }
}