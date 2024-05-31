using CommandsService.Models;

namespace CommandsService.DataService.Sync.Grpc.Interfaces
{
    public interface IPlatformDataClient
    {
         IEnumerable<Platform> ReturnAllPlatforms();
    }
}