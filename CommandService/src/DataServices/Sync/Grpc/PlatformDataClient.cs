using AutoMapper;
using CommandService.Models;
using CommandService.Settings;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.DataServices.Sync.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IDictionary<string, string> _variables;
        private readonly IMapper _mapper;

        public PlatformDataClient(IEnvironmentVariables environment, IMapper mapper)
        {
            _variables = environment.PlatformApi();
            _mapper = mapper;
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            Console.WriteLine($"--> Requesting platforms via gRPC");
            var channel = GrpcChannel.ForAddress(_variables["GrpcHost"]);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call gRPC server {ex.Message}");
                return null;
            }
        }
    }
}