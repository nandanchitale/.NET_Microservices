using CommandsService.Models;

namespace CommandsService.Data.IRepository;

public interface ICommandsRepository
{
    bool SaveChanges();

    // Platforms
    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    bool PlatFormExists(int id);
    bool ExternalPlatformExists(int externalPlatformId);

    // Commands
    IEnumerable<Command> GetCommandsForPlatform(int platformId);
    Command GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
}