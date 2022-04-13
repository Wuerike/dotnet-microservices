namespace PlatformService.Settings
{
    public class EnvironmentVariables : IEnvironmentVariables
    {
        private static string GetEnvironmentVariable(string name, string defaultValue)
            => Environment.GetEnvironmentVariable(name) is string v && v.Length > 0 ? v : defaultValue;

        public IDictionary<string, string> ApplicationVariables()
        {
            return new Dictionary<string, string>(){
                {"InMemoryDb", GetEnvironmentVariable("PLATFORM_API_IN_MEM_DB", "False")},
            };
        }

        public IDictionary<string, string> CommandApiVariables()
        {
            return new Dictionary<string, string>(){
                {"Url", GetEnvironmentVariable("PLATFORM_API_COMMAND_API_URL", "")},
            };
        }

        public IDictionary<string, string> DatabaseVariables()
        {
            var server = GetEnvironmentVariable("PLATFORM_API_SQL_SERVER_URL", "");
            var port = GetEnvironmentVariable("PLATFORM_API_SQL_SERVER_PORT", "");
            var user = GetEnvironmentVariable("PLATFORM_API_SQL_SERVER_USER", "");
            var password = GetEnvironmentVariable("PLATFORM_API_SQL_SERVER_PASSWORD", "");
            var database = GetEnvironmentVariable("PLATFORM_API_SQL_SERVER_DB_NAME", "");
            var connectionString = $"Server={server},{port}; Initial Catalog={database}; User ID={user}; Password={password}";

            return new Dictionary<string, string>(){
                {"ConnectionString", connectionString}
            };
        }

        public IDictionary<string, string> MessageBusVariables()
        {
            return new Dictionary<string, string>(){
                {"Host", GetEnvironmentVariable("PLATFORM_API_RABBITMQ_HOST", "")},
                {"Port", GetEnvironmentVariable("PLATFORM_API_RABBITMQ_PORT", "")},
                {"Exchange", GetEnvironmentVariable("PLATFORM_API_RABBITMQ_EXCHANGE", "")}
            };
        }
    }
}