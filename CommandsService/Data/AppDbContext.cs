using CommandsService.Models;
using Microsoft.EntityFrameworkCore;
namespace CommandsService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {
    }

    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Command> Commands { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);

        modelBuilder
        .Entity<Platform>()
        .HasMany(rec => rec.Commands)
        .WithOne(rec => rec.Platform)
        .HasForeignKey(rec => rec.PlatformId);

        modelBuilder
        .Entity<Command>()
        .HasOne(rec => rec.Platform)
        .WithMany(rec => rec.Commands)
        .HasForeignKey(rec => rec.PlatformId);

    }
}