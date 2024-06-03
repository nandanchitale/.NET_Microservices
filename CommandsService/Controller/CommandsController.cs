using AutoMapper;
using CommandsService.Data.IRepository;
using CommandsService.DTO;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CommandsService.Controllers;

[ApiController]
[Route("api/c/platforms/{platformId}/[controller]")]
public class CommandsController : ControllerBase
{
    // _repository is an instance of ICommandsRepository, which is a dependency injected
    // through the constructor. It provides data access to commands.
    private readonly ICommandsRepository _repository;

    // _mapper is an instance of IMapper, which is used for object-to-object mapping.
    // It's also dependency injected through the constructor.
    private readonly IMapper _mapper;

    public CommandsController(ICommandsRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // GetCommandsForPlatform is an HTTP GET method that retrieves a list of commands
    // for a given platform ID.
    [HttpGet]
    public IActionResult GetCommandsForPlatform(int platformId)
    {
        // Initialize response as NotFound, assuming the platform ID is invalid
        IActionResult response = NotFound();

        try
        {
            // Retrieve a list of commands for the given platform ID
            IEnumerable<Command> commands = null;

            Console.WriteLine($"--> Hit GetCommandsForPlatform : {platformId}");

            // Check if the platform ID exists in the repository
            if (_repository.PlatFormExists(platformId))
            {
                // Retrieve the list of commands for the platform ID
                commands = _repository.GetCommandsForPlatform(platformId);

                // Map the list of commands to a list of CommandReadDto objects
                response = Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
            }
        }
        catch (Exception ex)
        {
            // Log an error message to the console with a red background color
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at Commands > GetCommandsForPlatform() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }

        return response;
    }

    // GetCommandForPlatform is an HTTP GET method that retrieves a single command
    // for a given platform ID and command ID.
    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public IActionResult GetCommandForPlatform(int platformId, int commandId)
    {
        // Initialize response as NotFound, assuming the platform ID or command ID is invalid
        IActionResult response = NotFound();

        try
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform : {platformId} / {commandId}");

            // Check if the platform ID exists in the repository
            if (_repository.PlatFormExists(platformId))
            {
                // Retrieve the command for the given platform ID and command ID
                Command command = _repository.GetCommand(platformId, commandId);

                if (command is not null)
                {
                    // Map the command to a CommandReadDto object
                    response = Ok(_mapper.Map<CommandReadDto>(command));
                }
            }
        }
        catch (Exception ex)
        {
            // Log an error message to the console with a red background color
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at Commands > GetCommandForPlatform() => {ex.Message}\n{ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
        }

        return response;
    }

    // CreateCommandForPlatform is an HTTP POST method that creates a new command
    // for a given platform ID.
    [HttpPost]
    public IActionResult CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
    {
        // Initialize response as NotFound, assuming the platform ID is invalid
        IActionResult response = NotFound();

        try
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform : {platformId}");

            Console.WriteLine($"--> Command : {commandDto.HowTo} | {commandDto.CommandLine}");

            Console.WriteLine($"--> _repository.PlatFormExists(platformId) : {_repository.PlatFormExists(platformId)}");

            // Check if the platform ID exists in the repository
            if (_repository.PlatFormExists(platformId))
            {
                // Map the CommandCreateDto object to a Command object
                Command command = _mapper.Map<Command>(commandDto);

                Console.WriteLine($"Command: {JsonSerializer.Serialize(command)}");

                // Create the command in the repository
                _repository.CreateCommand(platformId, command);

                // Save changes to the repository
                _repository.SaveChanges();

                Console.WriteLine($"--> Command Created for platform ID {platformId}");

                // Map the created command to a CommandReadDto object
                CommandReadDto commandReadDto = _mapper.Map<CommandReadDto>(command);

                // Return a 201 Created response with the created command
                response = CreatedAtRoute(
                    nameof(GetCommandForPlatform),
                    new
                    {
                        platformId = platformId,
                        commandId = commandReadDto.Id
                    },
                    commandReadDto
                );
            }
        }
        catch (Exception ex)
        {
            // Log an error message to the console with a red background color
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at Commands > CreateCommandForPlatform() => {ex.Message}\nStacktrace : {ex.StackTrace}");
            Console.BackgroundColor = ConsoleColor.Black;
            response = BadRequest();
        }
        return response;
    }
}