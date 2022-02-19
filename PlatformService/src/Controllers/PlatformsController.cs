using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/platforms")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;

        public PlatformsController(IPlatformRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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

        [HttpPost] // POST /items
        public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto request)
        {
            var p = _mapper.Map<Platform>(request);

            _repository.CreatePlatform(p);
            _repository.SaveChanges();

            return CreatedAtAction(nameof(GetPlatformById), new{id = p.Id}, _mapper.Map<PlatformReadDto>(p));
        }
    }
}