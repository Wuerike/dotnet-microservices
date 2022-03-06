namespace PlatformService.Settings
{
    public class EnvironmentVariables : IEnvironmentVariables
    {
        public string GetConnectionString()
        {
            var server = Environment.GetEnvironmentVariable("PLATFORM_SQL_SERVER_URL");
            var port = Environment.GetEnvironmentVariable("PLATFORM_SQL_SERVER_PORT");
            var user = Environment.GetEnvironmentVariable("PLATFORM_SQL_SERVER_USER");
            var password = Environment.GetEnvironmentVariable("PLATFORM_SQL_SERVER_PASSWORD");
            var database = Environment.GetEnvironmentVariable("PLATFORM_SQL_SERVER_DB_NAME");
            return $"Server={server},{port}; Initial Catalog={database}; User ID={user}; Password={password}";
        }

        public string GetCommandServiceUrl()
        {
            var baseUrl = Environment.GetEnvironmentVariable("COMMAND_API_URL");
            return baseUrl;
        }
    }
}