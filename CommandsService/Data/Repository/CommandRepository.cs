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

    public bool SaveChanges()
    {
        bool returnValue = false;
        try
        {
            returnValue = _dbContext.SaveChanges() >= 0;
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > SaveChanges() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return returnValue;
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        IEnumerable<Platform> platforms = null;
        try
        {
            platforms = _dbContext.Platforms.ToList();
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > GetAllPlatforms() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return platforms;
    }

    public void CreatePlatform(Platform platform)
    {
        try
        {
            if (platform is null) throw new ArgumentNullException(nameof(platform));
            _dbContext.Platforms.Add(platform);
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > CreatePlatform() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    public bool PlatFormExists(int id)
    {
        bool returnValue = false;
        try
        {
            returnValue = _dbContext.Platforms.Any(rec => rec.Id.Equals(id));
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > PlatFormExists() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return returnValue;
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        IEnumerable<Command> commands = null;
        try
        {
            commands = _dbContext
                        .Commands
                        .Where(rec => rec.PlatformId.Equals(platformId))
                        .OrderBy(rec => rec.Platform.Name);
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > GetCommandsForPlatform() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return commands;
    }

    public Command GetCommand(int platformId, int commandId)
    {
        Command command = null;
        try
        {
            command = _dbContext
                        .Commands
                        .Where(
                            rec => rec.PlatformId.Equals(platformId) &&
                            rec.Id.Equals(commandId)
                        )
                        .FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > GetCommand() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return null;
    }

    public void CreateCommand(int platformId, Command command)
    {
        try
        {
            if (command is null) throw new ArgumentNullException(nameof(command));
            command.PlatformId = platformId;
            _dbContext.Commands.Add(command);
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > GetCommandForPlatform() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    public bool ExternalPlatformExists(int externalPlatformId)
    {
        bool returnValue = false;
        try
        {
            returnValue = _dbContext.Platforms.Any(
                                rec => rec.ExternalId.Equals(externalPlatformId)
                            );
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > ExternalPlatformExists() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return returnValue;
    }
}