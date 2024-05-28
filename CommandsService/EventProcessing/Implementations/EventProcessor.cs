using CommandsService.EventProcessing.Interfaces;

namespace CommandsService.EventProcessing.Implementations;

enum EventType{
    PlatformPublished,
    Undetermined
}

public class EventProcessor : IEventProcessor
{
    public void ProcessEvent(string message)
    {
        throw new NotImplementedException();
    }
}