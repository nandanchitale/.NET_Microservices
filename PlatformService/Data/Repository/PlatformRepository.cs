using PlatformService.Data.IRepository;
using PlatformService.Models;

namespace PlatformService.Data.Repository;

public class PlatformRepository : IPlatformRepository
{
    private readonly AppDbContext _context;

    public PlatformRepository(AppDbContext context)
    {
        _context = context;
    }
    public void CreatePlatform(Platform platform)
    {
        if (platform == null) throw new ArgumentNullException(nameof(platform));

        _context.Add(platform);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public Platform? GetPlatformById(int id)
    {
        return _context.Platforms.Where(rec => rec.Id.Equals(id)).FirstOrDefault();
    }

    public bool SaveChanges()
    {
        return (_context.SaveChanges() >= 0);
    }
}