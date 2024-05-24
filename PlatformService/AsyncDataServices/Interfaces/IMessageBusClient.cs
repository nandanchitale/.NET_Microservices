using PlatformService.DTO;

namespace PlatformService.AsyncDataServices.Interfaces;

public interface IMessageBusClient
{
    void PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
}
