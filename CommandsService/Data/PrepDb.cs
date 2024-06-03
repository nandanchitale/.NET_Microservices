using CommandsService.Data.IRepository;
using CommandsService.DataService.Sync.Grpc.Interfaces;
using CommandsService.Models;

namespace CommandsService.Data;

public class PrepareDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {
        try
        {
            using (IServiceScope serviceScope = app.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
                IEnumerable<Platform> platforms = grpcClient.ReturnAllPlatforms();
                SeedData(serviceScope.ServiceProvider.GetService<ICommandsRepository>(), platforms);
            }
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at PrepareDb > PrepPopulation() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    private static void SeedData(
        ICommandsRepository repository,
        IEnumerable<Platform> platforms
    )
    {
        try
        {
            Console.WriteLine("Seeding new Platforms...");
            foreach (Platform platform in platforms)
            {
                if (!repository.ExternalPlatformExists(platform.ExternalId)) 
                    repository.CreatePlatform(platform);
            }
            repository.SaveChanges();
        }
        catch (Exception ex)
        {
            // Logs an error message to the console with a red background color.
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Exception at GetPlatforms() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}