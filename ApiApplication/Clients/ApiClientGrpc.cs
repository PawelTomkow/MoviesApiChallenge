using System;
using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Clients.Contracts;
using ApiApplication.Configurations;
using ApiApplication.Core.Exceptions;
using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ProtoDefinitions;

namespace ApiApplication.Clients
{
    public class ApiClientGrpc : IApiClient
    {
        private readonly IMapper _mapper;
        private readonly MoviesApi.MoviesApiClient _client;

        [ActivatorUtilitiesConstructor]
        public ApiClientGrpc(IOptions<ApiClientConfiguration> apiClientOptions, IMapper mapper)
        {
            _mapper = mapper;
            _client = BuildMoviesApiClient(apiClientOptions?.Value);
        }

        public ApiClientGrpc(IOptions<ApiClientConfiguration> apiClientOptions, IMapper mapper, MoviesApi.MoviesApiClient client)
        {
            _mapper = mapper;
            _client = client;
        }

        public async Task<ShowResponse> GetByIdAsync(string id)
        {
            var request = new IdRequest()
            {
                Id = id
            };

            var response = await _client.GetByIdAsync(request);
            if (response.Success)
            {
                response.Data.TryUnpack<showResponse>(out var data);
                return _mapper.Map<ShowResponse>(data);
            }
            
            throw new ResourceUnavailableException(response.Exceptions.ToString());
        }

        public async Task<ShowListResponse> SearchAsync(string text)
        {
            var request = new SearchRequest()
            {
                Text = text
            };

            var response = await _client.SearchAsync(request);
            if (response.Success)
            {
                response.Data.TryUnpack<showListResponse>(out var data);
                return _mapper.Map<ShowListResponse>(data);
            }
            
            throw new ResourceUnavailableException(response.Exceptions.ToString());
        }

        public async Task<ShowListResponse> GetAllAsync()
        {
            var response = await _client.GetAllAsync(new Empty());
            if (response.Success)
            {
                response.Data.TryUnpack<showListResponse>(out var data);
                return _mapper.Map<ShowListResponse>(data);
            }

            throw new ResourceUnavailableException(response.Exceptions.ToString());
        }
        
        private MoviesApi.MoviesApiClient BuildMoviesApiClient(ApiClientConfiguration configuration)
        {
            var httpHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            
            
            var callCredentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (!string.IsNullOrEmpty(configuration.ApiKey))
                {
                    metadata.Add("X-Apikey", $"{configuration.ApiKey}");
                }
                return Task.CompletedTask;
            });

            var channel =
                GrpcChannel.ForAddress(configuration.BaseAddress, new GrpcChannelOptions()
                {
                    HttpHandler = httpHandler,
                    Credentials = ChannelCredentials.Create(new SslCredentials(), callCredentials),
                    
                });
            var client = new MoviesApi.MoviesApiClient(channel);
            return client;
        }
    }
}