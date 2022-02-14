using AutoMapper;
using PlatformApi.Dtos;
using PlatformApi.Models;

namespace PlatformApi.Profiles
{
    public class PlatformsProfile : Profile
    {
        public PlatformsProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformCreateDto, Platform>();
        }
    }
}