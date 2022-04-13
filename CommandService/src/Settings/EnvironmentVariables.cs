namespace CommandService.Settings
{
    public class EnvironmentVariables : IEnvironmentVariables
    {
        private static string GetEnvironmentVariable(string name, string defaultValue)
            => Environment.GetEnvironmentVariable(name) is string v && v.Length > 0 ? v : defaultValue;

        public IDictionary<string, string> PlatformApiVariables()
        {
            var platformApiVariables = new Dictionary<string, string>(){
                {"GrpcHost", GetEnvironmentVariable("COMMAND_API_PLATFORM_API_GRPC", "")},
            };

            return platformApiVariables;
        }

        public IDictionary<string, string> MessageBusVariables()
        {
            var messageBusVariables = new Dictionary<string, string>(){
                {"Host", GetEnvironmentVariable("COMMAND_API_RABBITMQ_HOST", "")},
                {"Port", GetEnvironmentVariable("COMMAND_API_RABBITMQ_PORT", "")},
                {"Exchange", GetEnvironmentVariable("COMMAND_API_RABBITMQ_EXCHANGE", "")}
            };

            return messageBusVariables;
        }
    }
}