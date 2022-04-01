namespace CommandService.Settings
{
    public class EnvironmentVariables : IEnvironmentVariables
    {
        public static string GetEnvironmentVariable(string name, string defaultValue)
            => Environment.GetEnvironmentVariable(name) is string v && v.Length > 0 ? v : defaultValue;

        public IDictionary<string, string> MessageBusVariables()
        {
            var messageBusVariables = new Dictionary<string, string>(){
                {"Host", GetEnvironmentVariable("RABBITMQ_HOST", "")},
                {"Port", GetEnvironmentVariable("RABBITMQ_PORT", "")}
            };

            return messageBusVariables;
        }
    }
}