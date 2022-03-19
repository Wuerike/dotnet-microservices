using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/c/platforms")]
    public class PlatformController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public PlatformController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet] // GET /platforms
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var p = _repository.GetAllPlatforms();

            if (!p.Any())
            {
                return NotFound();
            }

            return _mapper.Map<IEnumerable<PlatformReadDto>>(p).ToList();
        }
    }
}

