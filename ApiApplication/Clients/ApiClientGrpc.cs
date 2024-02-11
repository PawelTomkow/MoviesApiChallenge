using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Clients.Contracts;
using ApiApplication.Configurations;
using AutoMapper;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using ProtoDefinitions;

namespace ApiApplication.Clients
{
    public class ApiClientGrpc : IApiClient
    {
        private readonly IMapper _mapper;
        private readonly ApiClientConfiguration _apiClientOptions;

        public ApiClientGrpc(IOptions<ApiClientConfiguration> apiClientOptions, IMapper mapper)
        {
            _mapper = mapper;
            _apiClientOptions = apiClientOptions.Value;
        }

        public async Task<ShowResponse> GetByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ShowResponse> SearchAsync(string text)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ShowListResponse> GetAllAsync()
        {
            var client = BuildMoviesApiClient();

            var response = await client.GetAllAsync(new Empty());
            response.Data.TryUnpack<showListResponse>(out var data);
            return _mapper.Map<ShowListResponse>(data);
        }
        
        private static MoviesApi.MoviesApiClient BuildMoviesApiClient()
        {
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            var channel =
                GrpcChannel.ForAddress("https://localhost:7443", new GrpcChannelOptions()
                {
                    HttpHandler = httpHandler
                });
            var client = new MoviesApi.MoviesApiClient(channel);
            return client;
        }
    }
}