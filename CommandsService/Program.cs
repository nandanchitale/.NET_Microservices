using CommandsService.Data;
using CommandsService.Data.IRepository;
using CommandsService.Data.Repository;
using CommandsService.DataService.Async;
using CommandsService.EventProcessing.Implementations;
using CommandsService.EventProcessing.Interfaces;
using Helpers.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Commands Microservice API",
        Description = "An ASP.NET Core Web API for Commands Microservices",
    });

    options.ResolveConflictingActions(apiDescription => apiDescription.First());
});

// DBConnection
builder.Services.AddDbContext<AppDbContext>(
  option => option.UseInMemoryDatabase("InMem")
);

builder.Services.AddSingleton<RabbitMQHelper>();

// Command Repository for DI
builder.Services.AddScoped<ICommandsRepository, CommandRepository>();

builder.Services.AddControllers();

builder.Services.AddHostedService<MessageBusSubscriber>();

// Adding Evnet Processor As singleton to use it as message bus subscriber
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

app.Run();