using PlatformService.Models;

namespace PlatformService.Data;

public class PrepareDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using (var serviceScoped = app.ApplicationServices.CreateScope())
        {
            SeedData(serviceScoped.ServiceProvider.GetService<AppDbContext>());
        }
    }

    /// <summary>
    /// Method to seed all the data 
    /// </summary>
    /// <param name="context"></param>
    private static void SeedData(AppDbContext context)
    {
        if (!context.Platforms.Any())
        {
            Console.WriteLine("--> Seeding Data...");
            context.Platforms.AddRange(
                new Platform(){
                    Name = "Dot Net",
                    Publisher = "Microsoft",
                    Cost = "Free"
                },

                new Platform(){
                    Name = "Postgresql",
                    Publisher = "Postgresql",
                    Cost = "Free"
                },

                new Platform(){
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
}