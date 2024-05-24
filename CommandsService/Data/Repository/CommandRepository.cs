using CommandsService.Data.IRepository;
using CommandsService.Models;

namespace CommandsService.Data.Repository;

public class CommandRepository : ICommandsRepository
{
    private readonly AppDbContext _dbContext;

    public CommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool saveChanges()
    {
        return _dbContext.SaveChanges() >= 0;
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _dbContext.Platforms.ToList();
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform is null) throw new ArgumentNullException(nameof(platform));
        _dbContext.Platforms.Add(platform);
    }

    public bool PlatFormExists(int id)
    {
        return _dbContext.Platforms.Any(rec => rec.Id.Equals(id));
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _dbContext
                .Commands
                .Where(rec => rec.PlatformId.Equals(platformId))
                .OrderBy(rec => rec.Platform.Name);
    }

    public Command GetCommand(int platformId, int commandId)
    {
        return _dbContext
                .Commands
                .Where(
                    rec => rec.PlatformId.Equals(platformId) &&
                    rec.Id.Equals(commandId)
                )
                .FirstOrDefault();

    }

    public void CreateCommand(int platformId, Command command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));
        command.PlatformId = platformId;
        _dbContext.Commands.Add(command);
    }
}