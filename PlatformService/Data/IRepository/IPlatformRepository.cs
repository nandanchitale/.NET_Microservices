using PlatformService.Models;

namespace PlatformService.Data.IRepository;

public interface IPlatformRepository
{
    bool SaveChanges();

    IEnumerable<Platform> GetAllPlatforms();
    Platform? GetPlatformById(int id);
    void CreatePlatform(Platform platform);
}