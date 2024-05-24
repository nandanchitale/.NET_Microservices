using AutoMapper;
using CommandsService.DTO;
using CommandsService.Models;

namespace CommandsService.Profiles;

public class Commandsprofile : Profile
{
  public Commandsprofile()
  {
    // Source -> target
    CreateMap<Platform, PlatformReadDto>();
    CreateMap<CommandCreateDto, Command>();
    CreateMap<Command, CommandReadDto>();
  }
}
