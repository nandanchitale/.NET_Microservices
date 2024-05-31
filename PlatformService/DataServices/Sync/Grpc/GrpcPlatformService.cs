using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Http.HttpResults;
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
            Task<PlatformResponse> response = null;
            try
            {
                PlatformResponse platformResponse = new PlatformResponse();
                IEnumerable<Platform> platforms = _platformRepository.GetAllPlatforms();

                foreach (Platform platform in platforms)
                {
                    platformResponse.Platform.Add(
                        _mapper.Map<GrpcPlatformModel>(platform)
                    );
                }
                response = Task.FromResult(platformResponse);
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"--> Exception at GetPlatforms() => {ex.Message}");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            return response;
        }
    }
}