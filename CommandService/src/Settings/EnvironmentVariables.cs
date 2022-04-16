using CommandService.Settings.Models;

namespace CommandService.Settings
{
    public class EnvironmentVariables : IEnvironmentVariables
    {
        private readonly IConfiguration _configuration;

        public EnvironmentVariables(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public PlatformApiVariables PlatformApiVariables()
        {
            return _configuration.GetSection("PlatformApi").Get<PlatformApiVariables>();
        }


        public MessageBusVariables MessageBusVariables()
        {
            return _configuration.GetSection("MessageBus").Get<MessageBusVariables>();
        }
    }
}