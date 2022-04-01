using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using PlatformService.Settings;

namespace PlatformService.DataServices.Sync.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public HttpCommandDataClient(HttpClient httpClient, IEnvironmentVariables envVars)
        {
            _httpClient = httpClient;
            _baseUrl = envVars.GetCommandServiceUrl();
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
                Console.WriteLine("----> Teste deu bom");
            }
            else
            {
                Console.WriteLine("----> Teste deu ruim");
            }
        }
    }
}