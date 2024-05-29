using AutoMapper;
using Grpc.Core;
using Microsoft.CodeAnalysis;
using PlatformService.Data.IRepository;
using Platform = PlatformService.Models.Platform;

namespace PlatformService.DataServices.Sync.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;

        public GrpcPlatformService(
            IPlatformRepository platformRepository,
            IMapper mapper
        )
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(
            GetAllRequest request,
            ServerCallContext context
        )
        {
            try
            {
                PlatformResponse response = new PlatformResponse();
                IEnumerable<Platform> platforms = _platformRepository.GetAllPlatforms();

                foreach(Platform platform in platforms){
                    response.Platform.Add(
                        _mapper.Map<GrpcPlatformModel>(platform)
                    );
                }
                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"--> Exception at GetPlatforms() => {ex.Message}");
                Console.BackgroundColor = ConsoleColor.Black;
                return null;
            }
        }
    }
}