using CommandsService.Models;

namespace CommandsService.Data.IRepository;

public interface ICommandsRepository
{
    // Saves changes to the repository.
    bool SaveChanges();

    // Platforms

    // Retrieves all platforms from the repository.
    IEnumerable<Platform> GetAllPlatforms();

    // Creates a new platform in the repository.
    void CreatePlatform(Platform platform);

    // Checks if a platform with the given ID exists in the repository.
    bool PlatFormExists(int id);

    // Checks if an external platform with the given ID exists in the repository.
    bool ExternalPlatformExists(int externalPlatformId);

    // Commands

    // Retrieves a list of commands for a given platform ID from the repository.
    IEnumerable<Command> GetCommandsForPlatform(int platformId);

    // Retrieves a command for a given platform ID and command ID from the repository.
    Command GetCommand(int platformId, int commandId);

    // Creates a new command for a given platform ID in the repository.
    void CreateCommand(int platformId, Command command);
}