using Helpers.RabbitMq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PlatformService;
using PlatformService.AsyncDataServices.Implementation;
using PlatformService.AsyncDataServices.Interfaces;
using PlatformService.Data;
using PlatformService.Data.IRepository;
using PlatformService.Data.Repository;
using PlatformService.DataServices.Sync.Grpc;
using PlatformService.SyncDataServices.Implementation;
using PlatformService.SyncDataServices.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Platforms Microservice API",
        Description = "An ASP.NET Core Web API for Platform Microservices",
    });

    options.ResolveConflictingActions(apiDescription => apiDescription.First());
});

// Setup SQL server database
if (builder.Environment.IsProduction())
{
    Console.WriteLine($"--> Application Environment IsProduction ? {builder.Environment.IsProduction()}");
    Console.WriteLine("--> Using SQL Server DB");
    Console.WriteLine($"--> Connection String : {builder.Configuration.GetConnectionString("PlatformsConnection")}");
    _ = builder.Services.AddDbContext<AppDbContext>(
        opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConnection"))
    );
}
// Setup in memory database
else
{
    Console.WriteLine("--> Using InMem DB");
    _ = builder.Services.AddDbContext<AppDbContext>(
        opt => opt.UseInMemoryDatabase("InMemoryDb")
    );
}

// Grpc
builder.Services.AddGrpc();

builder.Services.AddSingleton<RabbitMQHelper>();

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

// DI for messgae bus 
// This is singleton because we want to use same connection throught all application
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddControllers();

// Add Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Dependancy Injection
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    _ = app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = String.Empty;
    });

    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    _ = app.MapControllers();
    _ = endpoints.MapGrpcService<GrpcPlatformService>();

    // optional
    _ = endpoints.MapGet(
        "/protos/platforms.proto",
        async context =>
        {
            await context.Response.WriteAsync(
                File.ReadAllText("Protos/platforms.proto")
            );
        }
    );
});

PrepareDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();