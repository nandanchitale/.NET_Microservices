using AutoMapper;
using CommandsService.Data.IRepository;
using CommandsService.DTO;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[ApiController]
[Route("api/c/platforms/{platformId}/[controller]")]
public class CommandsController : ControllerBase
{
    private readonly ICommandsRepository _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandsRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetCommandsForPlatform(int platformId)
    {
        IActionResult response = NotFound();
        try
        {
            IEnumerable<Command> commands = null;
            Console.WriteLine($"--> Hit GetCommandsForPlatform : {platformId}");
            if (_repository.PlatFormExists(platformId))
            {
                commands = _repository.GetCommandsForPlatform(platformId);
                response = Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
            }
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at Commands > GetCommandsForPlatform() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return response;
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public IActionResult GetCommandForPlatform(int platformId, int commandId)
    {
        IActionResult response = NotFound();
        try
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform : {platformId} / {commandId}");
            if (_repository.PlatFormExists(platformId))
            {
                Command command = _repository.GetCommand(platformId, commandId);
                if (command is not null)
                {
                    response = Ok(
                        _mapper.Map<CommandReadDto>(command)
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at Commands > GetCommandForPlatform() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return response;
    }

    [HttpPost]
    public IActionResult CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
    {
        IActionResult response = NotFound();
        try
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform : {platformId}");
            if (_repository.PlatFormExists(platformId))
            {
                Command command = _mapper.Map<Command>(commandDto);
                _repository.CreateCommand(platformId, command);
                _repository.saveChanges();

                CommandReadDto commandReadDto = _mapper.Map<CommandReadDto>(command);
                response = CreatedAtRoute(
                    nameof(GetCommandForPlatform),
                    new {
                        platformId = platformId,
                        commandId = commandReadDto.Id
                    }, 
                    commandReadDto
                );
            }
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at Commands > CreateCommandForPlatform() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return response;
    }
}