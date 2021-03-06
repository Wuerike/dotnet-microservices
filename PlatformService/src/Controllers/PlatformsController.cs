using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.DataServices.Sync.Http;
using PlatformService.DataServices.Async;
using Serilog;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/p/platforms")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet] // GET /platformts
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var p = _repository.GetAllPlatforms();

            if (!p.Any())
            {
                return NotFound();
            }

            return _mapper.Map<IEnumerable<PlatformReadDto>>(p).ToList();
        }

        [HttpGet("{id}")] // GET /platformts/{id}
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var p = _repository.GetPlatformById(id);

            if (p is null)
            {
                return NotFound();
            }

            return _mapper.Map<PlatformReadDto>(p);
        }

        [HttpPost] // POST /platformts
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto request)
        {
            var p = _mapper.Map<Platform>(request);

            _repository.CreatePlatform(p);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(p);
            
            try
            {
                // Will always fail, the Command endpoint doesnt exists
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Log.Error($"Internal error while posting to command api: {ex.Message}");
            }

            try
            {
                var platformPublishDto = _mapper.Map<PlatformPublishDto>(platformReadDto);
                platformPublishDto.Event = "platform_publish";
                _messageBusClient.PublishNewPlatform(platformPublishDto);
            }
            catch (Exception ex)
            {
                Log.Error($"Could not send the platform over the message bus: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetPlatformById), new{id = p.Id}, platformReadDto);
        }
    }
}