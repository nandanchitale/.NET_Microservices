using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[ApiController]
[Route("api/c/[controller]/[action]")]
public class PlatformsController : ControllerBase
{
    public PlatformsController()
    {

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
}