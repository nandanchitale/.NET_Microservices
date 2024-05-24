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
}