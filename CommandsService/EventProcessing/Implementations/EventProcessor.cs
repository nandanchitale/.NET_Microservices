using AutoMapper;
using CommandsService.Data.IRepository;
using CommandsService.DTO;
using CommandsService.EventProcessing.Interfaces;
using CommandsService.Models;
using Helpers.Constants.Events;
using System.Text.Json;

namespace CommandsService.EventProcessing.Implementations;

enum EventType
{
    PlatformPublished,
    Undetermined
}

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;


    public EventProcessor(
        IServiceScopeFactory scopeFactory,
        IMapper mapper
    )
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
        try
        {
            EventType eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at EventProcessor > ProcessEvent() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    /// <summary>
    /// Method to Add event to database
    /// </summary>
    /// <param name="message"></param>
    private void AddPlatform(string platformPubliedMessage)
    {
        try
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                ICommandsRepository repository = scope.ServiceProvider.GetRequiredService<ICommandsRepository>();

                PlatformPublishDto platformPublishDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPubliedMessage);

                Platform platform = _mapper.Map<Platform>(platformPublishDto);

                if (!repository.ExternalPlatformExists(platform.ExternalId))
                {
                    repository.CreatePlatform(platform);
                    repository.SaveChanges();
                    Console.WriteLine("--> Platform Added Successfully");
                }
                else
                {
                    Console.WriteLine("--> Platform Already Exists");
                }
            };
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at EventProcessor > AddPlatform() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        EventType returnEventType = EventType.Undetermined;
        try
        {
            Console.WriteLine("--> Determining Event");
            GenericEventDto eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch (eventType.Event)
            {
                case PlatformEvents.PLATFORM_PUBLISHED:
                    Console.WriteLine("--> Platform Publied Event Detected");
                    returnEventType = EventType.PlatformPublished;
                    break;
                default:
                    Console.WriteLine("--> Could not determine event type");
                    returnEventType = EventType.Undetermined;
                    break;
            }
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at EventProcessor > DetermineEvent() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        return returnEventType;
    }
}