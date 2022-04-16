using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using PlatformService.Settings;
using Serilog;

/*
    Example of an HTTP Client
    The errors logs are always expected because
    The post endpoint is not implemented in the Command API
*/
namespace PlatformService.DataServices.Sync.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public HttpCommandDataClient(HttpClient httpClient, IEnvironmentVariables environment)
        {
            _httpClient = httpClient;
            _baseUrl = environment.CommandApiVariables()["Url"];
        }

        public async Task SendPlatformToCommand(PlatformReadDto p)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(p),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                $"{_baseUrl}/api/c/platforms",
                httpContent
            );

            if (response.IsSuccessStatusCode)
            {
                Log.Information("Command API request succeeded");
            }
            else
            {
                Log.Error("Command API request failed");
            }
        }
    }
}