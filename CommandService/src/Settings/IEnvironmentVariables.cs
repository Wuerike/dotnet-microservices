namespace CommandService.Settings
{
    public interface IEnvironmentVariables
    {
        IDictionary<string, string> MessageBusVariables();
    }
}