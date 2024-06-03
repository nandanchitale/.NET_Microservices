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
    // _repository is an instance of ICommandsRepository, which is a dependency injected
    // through the constructor. It provides data access to commands.
    private readonly ICommandsRepository _repository;

    // _mapper is an instance of IMapper, which is used for object-to-object mapping.
    // It's also dependency injected through the constructor.
    private readonly IMapper _mapper;

    public PlatformsController(
        ICommandsRepository commandsRepository,
        IMapper mapper
    )
    {
        _repository = commandsRepository;
        _mapper = mapper;
    }

    // TestInboundConnection is an HTTP POST method that tests the inbound connection.
    [HttpPost(Name = "TestConnection")]
    public IActionResult TestInboundConnection()
    {
        // Initialize the result as a Bad Request response.
        IActionResult result = BadRequest();

        try
        {
            // Write a message to the console indicating that an inbound POST request has been received.
            Console.WriteLine("--> Inbound POST # Command Service");

            // Set the result to an Okay response with a test message.
            result = Ok("Inbound test from Platform Controller");
        }
        catch (Exception ex)
        {
            // Log an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at Platforms > TestInboundConnection() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return result;
    }

    // GetPlatforms is an HTTP GET method that retrieves a list of platforms.
    [HttpGet]
    public IActionResult GetPlatforms()
    {
        // Initialize the response as a Bad Request response.
        IActionResult response = BadRequest();

        try
        {
            // Write a message to the console indicating that a GET request has been received.
            Console.WriteLine("--> Getting all Platforms from Command Service");

            // Retrieve a list of platforms from the repository.
            IEnumerable<Platform> platforms = _repository.GetAllPlatforms();

            // Map the list of platforms to a list of PlatformReadDto objects.
            response = Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }
        catch (Exception ex)
        {
            // Log an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at Platforms > GetPlatforms() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return response;
    }
}