using Helpers.RabbitMq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PlatformService.AsyncDataServices.Implementation;
using PlatformService.AsyncDataServices.Interfaces;
using PlatformService.Data;
using PlatformService.Data.IRepository;
using PlatformService.Data.Repository;
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
    builder.Services.AddDbContext<AppDbContext>(
        opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConnection"))
    );
}
// Setup in memory database
else
{
    Console.WriteLine("--> Using InMem DB");
    builder.Services.AddDbContext<AppDbContext>(
        opt => opt.UseInMemoryDatabase("InMemoryDb")
    );
}

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

    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = String.Empty;
    });

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

PrepareDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();