using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Serilog;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory= scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetemineEvent(message);

            if (eventType != EventType.PlatformPublish)
            {
                return;
            }

            AddPlatform(message);
            return;
        }

        private EventType DetemineEvent(string message)
        {
            Log.Information("Determing event type");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(message);

            switch(eventType.Event)
            {
                case "platform_publish":
                    Log.Information("Platform publish event detected");
                    return EventType.PlatformPublish;
                default:
                    Log.Warning("Could not determine the event type");
                    return EventType.Undertemined;
            }
        }


        private void AddPlatform(string message){
            using(var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var publishedPlatform = JsonSerializer.Deserialize<PlatformPublishDto>(message);

                try
                {
                    var platform = _mapper.Map<Platform>(publishedPlatform);

                    if(!repo.PlatformExists(platform.Id))
                    {
                        repo.CreatePlatform(platform);
                        repo.SaveChanges();
                    }
                    else
                    {
                        Log.Warning("Received platform already exists in database");
                    }
                    
                }
                catch (Exception)
                {
                    
                    Log.Error($"Could not add platform to database: {publishedPlatform}");
                }

            }
        }
    }

    enum EventType
    {
        PlatformPublish,
        Undertemined
    }
}