using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendPlatformToCommand(PlatformReadDto p)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(p),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                _configuration["CommandService"],
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