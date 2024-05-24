using PlatformService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data;

public class PrepareDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {
        using (IServiceScope serviceScope = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
        }
    }

    /// <summary>
    /// Method to seed all the data 
    /// </summary>
    /// <param name="context"></param>
    private static void SeedData(AppDbContext context, bool isProd)
    {
        try
        {
            if (isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations to SQL Server...");
                context.Database.Migrate();
                Console.WriteLine("--> Migration Applied to SQL Server...");
            }
            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data...");
                context.Platforms.AddRange(
                    new Platform()
                    {
                        Name = "Dot Net",
                        Publisher = "Microsoft",
                        Cost = "Free"
                    },

                    new Platform()
                    {
                        Name = "Postgresql",
                        Publisher = "Postgresql",
                        Cost = "Free"
                    },

                    new Platform()
                    {
                        Name = "Kubernetes",
                        Publisher = "Cloud Native Computing Foundation",
                        Cost = "Free"
                    }
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"Exception at SeedData() => {ex.Message}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}