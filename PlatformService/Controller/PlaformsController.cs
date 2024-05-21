using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data.IRepository;
using PlatformService.DTO;
using PlatformService.Models;

namespace PlatformService.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PlatformsController : ControllerBase
{

    private readonly IPlatformRepository _platform;
    private readonly IMapper _mapper;

    public PlatformsController(IPlatformRepository platform, IMapper mapper)
    {
        _mapper = mapper;
        _platform = platform;
    }

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
        }
        return returnValue;
    }

    [HttpPost]
    public ActionResult<PlatformReadDto> Create(PlatformCreateDto platformCreateDto)
    {
        ActionResult<PlatformReadDto> returnValue = NotFound();

        try
        {
            Platform PlatformModel = _mapper.Map<Platform>(platformCreateDto);
            _platform.CreatePlatform(PlatformModel);
            _platform.SaveChanges();

            PlatformReadDto platformReadDto = _mapper.Map<PlatformReadDto>(PlatformModel);
            return CreatedAtRoute(
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