namespace PlatformService.Settings
{
    public class EnvironmentVariables : IEnvironmentVariables
    {
        public static string GetEnvironmentVariable(string name, string defaultValue)
            => Environment.GetEnvironmentVariable(name) is string v && v.Length > 0 ? v : defaultValue;

        public string GetConnectionString()
        {
            var server = GetEnvironmentVariable("PLATFORM_SQL_SERVER_URL", "");
            var port = GetEnvironmentVariable("PLATFORM_SQL_SERVER_PORT", "");
            var user = GetEnvironmentVariable("PLATFORM_SQL_SERVER_USER", "");
            var password = GetEnvironmentVariable("PLATFORM_SQL_SERVER_PASSWORD", "");
            var database = GetEnvironmentVariable("PLATFORM_SQL_SERVER_DB_NAME", "");
            return $"Server={server},{port}; Initial Catalog={database}; User ID={user}; Password={password}";
        }

        public string GetCommandServiceUrl()
        {
            var baseUrl = GetEnvironmentVariable("COMMAND_API_URL", "");
            return baseUrl;
        }

        public bool IsInMemoryDb()
        {
            var IsInMemoryDb = GetEnvironmentVariable("PLATFORM_IS_IN_MEM_DB", "False");
            if(IsInMemoryDb.ToLower() == "true")
                return true;
            else
                return false;
        }
    }
}