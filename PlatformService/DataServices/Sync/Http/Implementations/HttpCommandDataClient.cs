using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PlatformService.DTO;
using PlatformService.SyncDataServices.Interfaces;

namespace PlatformService.SyncDataServices.Implementation;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    public HttpCommandDataClient(
        HttpClient httpClient,
        IConfiguration configuration
    )
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendPlatformToCommand(PlatformReadDto platform)
    {
        // IActionResult response = null;
        try
        {
            StringContent httpContent = new StringContent(
                JsonSerializer.Serialize(platform),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await _httpClient.PostAsync(
                $"{_configuration["CommandService"]}",
                httpContent
            );

            if (response.IsSuccessStatusCode) Console.WriteLine($"--> SYNC POST to command service was OK !");
            else Console.WriteLine($"--> SYNC POST to command service was FAIL !");
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at HttpCommandDataClient > SendPlatformToCommand() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}