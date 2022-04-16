using PlatformService.Settings.Models;

namespace PlatformService.Settings
{
    public class EnvironmentVariables : IEnvironmentVariables
    {
        private readonly IConfiguration _configuration;

        public EnvironmentVariables(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public CommandApiVariables CommandApiVariables()
        {
            return _configuration.GetSection("CommandApi").Get<CommandApiVariables>();
        }

        public DatabaseVariables DatabaseVariables()
        {
            return _configuration.GetSection("Database").Get<DatabaseVariables>();
        }

        public MessageBusVariables MessageBusVariables()
        {
            return _configuration.GetSection("MessageBus").Get<MessageBusVariables>();
        }
    }
}