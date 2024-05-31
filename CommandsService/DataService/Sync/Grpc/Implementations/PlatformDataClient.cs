using AutoMapper;
using CommandsService.DataService.Sync.Grpc.Interfaces;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;
using static PlatformService.GrpcPlatform;

namespace CommandsService.DataService.Sync.Grpc.Implementations
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public PlatformDataClient(
            IConfiguration configuration,
            IMapper mapper
        )
        {
            _configuration = configuration;
            _mapper = mapper;
        }
        public IEnumerable<Platform> ReturnAllPlatforms()
        {          
            IEnumerable<Platform> platforms = null;
            try
            {
                Console.WriteLine($"--> Calling GRPC Service {_configuration["GrpcPlatform"]}");
                var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"]);
                GrpcPlatformClient client = new GrpcPlatformClient(channel);
                GetAllRequest request = new GetAllRequest();
                var reply = client.GetAllPlatforms(request);
                platforms = _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call GRPC Server {ex.Message}");
            }
            return platforms;
        }
    }
}