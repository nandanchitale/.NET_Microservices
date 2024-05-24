using CommandsService.Data;
using CommandsService.Data.IRepository;
using CommandsService.Data.Repository;
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

// Command Repository for DI
builder.Services.AddScoped<ICommandsRepository, CommandRepository>();

builder.Services.AddControllers();

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
