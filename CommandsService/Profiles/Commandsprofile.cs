using AutoMapper;
using CommandsService.DTO;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles;

public class Commandsprofile : Profile
{
    public Commandsprofile()
    {
        // Source -> target
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<CommandCreateDto, Command>();
        CreateMap<Command, CommandReadDto>();
        CreateMap<PlatformPublishDto, Platform>()
        .ForMember(
            dest => dest.ExternalId, 
            opt => opt.MapFrom(src => src.Id)
        );
        CreateMap<GrpcPlatformModel, Platform>()
        .ForMember(
            dest => dest.ExternalId,
            opt => opt.MapFrom(src => src.PlatformId)
        )
        .ForMember(
            dest =>dest.Name,
            opt => opt.MapFrom(src => src.Name)
        )
        .ForMember(
            dest =>dest.Commands,
            opt => opt.Ignore()
        );
    }
}