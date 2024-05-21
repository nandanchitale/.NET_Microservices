using PlatformService.DTO;

namespace PlatformService.SyncDataServices.Interfaces;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto platform);
}