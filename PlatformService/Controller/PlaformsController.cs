using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data.IRepository;
using PlatformService.DTO;
using PlatformService.Models;
using PlatformService.SyncDataServices.Interfaces;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PlatformsController : ControllerBase
{

    private readonly IPlatformRepository _platform;
    private readonly IMapper _mapper;
    private ICommandDataClient _commandDataClient;

    public PlatformsController(
        IPlatformRepository platform,
        IMapper mapper,
        ICommandDataClient commandDataClient
    )
    {
        _mapper = mapper;
        _platform = platform;
        _commandDataClient = commandDataClient;
    }

    /// <summary>
    /// Method to Get List of all platforms
    /// </summary>
    /// <returns>Platforms Enumarable</returns>
    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> Get()
    {
        ActionResult<IEnumerable<PlatformReadDto>> returnValue = NotFound();

        try
        {
            IEnumerable<Platform> platforms = _platform.GetAllPlatforms();
            returnValue = Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at GetPlatforms() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;

            returnValue = BadRequest();
        }
        return returnValue;
    }

    /// <summary>
    /// Method to get Platform specified by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>One Specific Platform Read DTO</returns>
    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        ActionResult<PlatformReadDto> returnValue = NotFound();

        try
        {
            Platform platform = _platform.GetPlatformById(id);
            returnValue = platform != null ? Ok(_mapper.Map<PlatformReadDto>(platform)) : NotFound();
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at GetPlatforms() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
            returnValue = BadRequest();
        }
        return returnValue;
    }

    /// <summary>
    /// Method to create new platform
    /// </summary>
    /// <param name="platformCreateDto"></param>
    /// <returns>Newly created Platform Read DTO</returns>
    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> Create(PlatformCreateDto platformCreateDto)
    {
        ActionResult<PlatformReadDto> returnValue = BadRequest();

        try
        {
            Platform PlatformModel = _mapper.Map<Platform>(platformCreateDto);
            _platform.CreatePlatform(PlatformModel);
            _platform.SaveChanges();

            PlatformReadDto platformReadDto = _mapper.Map<PlatformReadDto>(PlatformModel);

            // Demo Code
            await _commandDataClient.SendPlatformToCommand(platformReadDto);

            returnValue = CreatedAtRoute(
                nameof(GetPlatformById),            // route name
                new { Id = platformReadDto.Id },      // route value
                platformReadDto                     // value
            );
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at GetPlatforms() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return returnValue;
    }
}