using AutoMapper;
using CommandsService.Data.IRepository;
using CommandsService.DTO;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[ApiController]
[Route("api/c/[controller]/[action]")]
public class PlatformsController : ControllerBase
{
    private readonly ICommandsRepository _repository;
    private readonly IMapper _mapper;

    public PlatformsController(
        ICommandsRepository commandsRepository,
        IMapper mapper
    )
    {
        _repository = commandsRepository;
        _mapper = mapper;
    }

    [HttpPost(Name = "TestConnection")]
    public IActionResult TestInboundConnection()
    {
        IActionResult result = BadRequest();
        try
        {
            Console.WriteLine("--> Inbound POST # Command Service");
            result = Ok("Inbound test from Platform Controller");
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at Platforms > TestInboundConnection() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return result;
    }

    [HttpGet]
    public IActionResult GetPlatforms()
    {
        IActionResult response = BadRequest();
        try
        {
            Console.WriteLine("--> Getting all Platforms from Command Service");
            IEnumerable<Platform> platforms = _repository.GetAllPlatforms();
            response = Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at Platforms > GetPlatforms() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return response;
    }
}