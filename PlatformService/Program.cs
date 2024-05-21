using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddControllers();

// Add Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Adding dbcontext 
builder.Services.AddDbContext<AppDbContext>(
    opt => opt.UseInMemoryDatabase("InMemoryDb")
);

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

PrepareDb.PrepPopulation(app);

app.Run();