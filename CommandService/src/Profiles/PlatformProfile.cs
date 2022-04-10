using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using PlatformService;

namespace CommandService.Profiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformPublishDto, Platform>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<GrpcPlatformModel, Platform>();
        }
    }
}