using AutoMapper;
using CommandService.Models;
using CommandService.Settings;
using Grpc.Net.Client;
using PlatformService;
using Serilog;

namespace CommandService.DataServices.Sync.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IDictionary<string, string> _variables;
        private readonly IMapper _mapper;

        public PlatformDataClient(IEnvironmentVariables environment, IMapper mapper)
        {
            _variables = environment.PlatformApiVariables();
            _mapper = mapper;
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            Log.Information("Requesting platforms via gRPC");
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
                Log.Error($"Could not call gRPC server {ex.Message}");
                return null;
            }
        }
    }
}