using CommandsService.Data.IRepository;
using CommandsService.Models;

namespace CommandsService.Data.Repository;

public class CommandRepository : ICommandsRepository
{
    // _dbContext is an instance of AppDbContext, which is used to interact with the database.
    private readonly AppDbContext _dbContext;

    // Constructor that initializes the _dbContext instance.
    public CommandRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Saves changes to the database.
    public bool SaveChanges()
    {
        bool returnValue = false;
        try
        {
            // Attempts to save changes to the database.
            returnValue = _dbContext.SaveChanges() >= 0;
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > SaveChanges() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return returnValue;
    }

    // Retrieves all platforms from the database.
    public IEnumerable<Platform> GetAllPlatforms()
    {
        IEnumerable<Platform> platforms = null;
        try
        {
            // Retrieves all platforms from the database using the _dbContext instance.
            platforms = _dbContext.Platforms.ToList();
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > GetAllPlatforms() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return platforms;
    }

    // Creates a new platform in the database.
    public void CreatePlatform(Platform platform)
    {
        try
        {
            // Checks if the platform object is null, and throws an ArgumentNullException if true.
            if (platform is null) throw new ArgumentNullException(nameof(platform));

            // Adds the platform to the database using the _dbContext instance.
            _dbContext.Platforms.Add(platform);
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > CreatePlatform() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    // Checks if a platform with the given ID exists in the database.
    public bool PlatFormExists(int id)
    {
        bool returnValue = false;
        try
        {
            // Checks if a platform with the given ID exists in the database using the _dbContext instance.
            returnValue = _dbContext.Platforms.Any(rec => rec.Id.Equals(id));
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > PlatFormExists() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return returnValue;
    }

    // Retrieves a list of commands for a given platform ID from the database.
    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        IEnumerable<Command> commands = null;
        try
        {
            // Retrieves a list of commands for the given platform ID from the database using the _dbContext instance.
            commands = _dbContext
                       .Commands
                       .Where(rec => rec.PlatformId.Equals(platformId))
                       .OrderBy(rec => rec.Platform.Name);
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > GetCommandsForPlatform() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return commands;
    }

    // Retrieves a command for a given platform ID and command ID from the database.
    public Command GetCommand(int platformId, int commandId)
    {
        Command command = null;
        try
        {
            // Retrieves a command for the given platform ID and command ID from the database using the _dbContext instance.
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
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > GetCommand() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return command;
    }

    // Creates a new command for a given platform ID in the database.
    public void CreateCommand(int platformId, Command command)
    {
        try
        {
            // Checks if the command object is null, and throws an ArgumentNullException if true.
            if (command is null) throw new ArgumentNullException(nameof(command));
            command.PlatformId = platformId;

            // Adds the Command to the database using the _dbContext instance.
            _dbContext.Commands.Add(command);
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
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
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at CommandRepository > ExternalPlatformExists() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return returnValue;
    }
}