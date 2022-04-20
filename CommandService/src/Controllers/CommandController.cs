using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/c/platforms/{platformId}/commands")]
    public class CommandController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet] // GET /platforms/{platformId}/commands
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsByPlatformId(int platformId)
        {
            
            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var c = _repository.GetCommandsByPlatformId(platformId);

            return _mapper.Map<IEnumerable<CommandReadDto>>(c).ToList();
        }

        [HttpGet("{commandId}")] // GET /platforms/{platformId}/commands/{commandId}
        public ActionResult<CommandReadDto> GetCommand(int platformId, int commandId)
        {

            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var c = _repository.GetCommand(commandId, platformId);

            if (c is null)
            {
                return NotFound();
            }

            return _mapper.Map<CommandReadDto>(c);
        }
    
        [HttpPost] // POST /platforms/{platformId}/commands
        public ActionResult<CommandReadDto> CreateCommand(int platformId, CommandCreateDto requestBody)
        {

            if (!_repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var c = _mapper.Map<Command>(requestBody);

            _repository.CreateCommand(platformId, c);
            _repository.SaveChanges();

            var createdCommand = _mapper.Map<CommandReadDto>(c);

            return CreatedAtAction(nameof(GetCommand), new{platformId = createdCommand.PlatformId, commandId = createdCommand.Id}, createdCommand);
        }
    }
}

