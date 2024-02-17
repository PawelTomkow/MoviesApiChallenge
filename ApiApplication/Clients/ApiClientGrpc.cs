using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Clients.Cache;
using ApiApplication.Clients.Contracts;
using ApiApplication.Configurations;
using ApiApplication.Core.Exceptions;
using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProtoDefinitions;

namespace ApiApplication.Clients
{
    public class ApiClientGrpc : IApiClient
    {
        private readonly IMapper _mapper;
        private readonly ICacheRepository _cacheRepository;
        private readonly ILogger<ApiClientGrpc> _logger;
        private readonly MoviesApi.MoviesApiClient _client;
        private const string Name = "ApiClient";

        [ActivatorUtilitiesConstructor]
        public ApiClientGrpc(IOptions<ApiClientConfiguration> apiClientOptions, 
            IMapper mapper,
            ICacheRepository cacheRepository,
            ILogger<ApiClientGrpc> logger)
        {
            var clientConfiguration = apiClientOptions.Value;
            
            if (clientConfiguration is null)
            {
                throw new ConfigurationException("Can not load ApiClientConfiguration.");
            }

            if (string.IsNullOrWhiteSpace(clientConfiguration.BaseAddress))
            {
                throw new ConfigurationException($"BaseAddress can not be null or empty");
            }
            
            _mapper = mapper;
            _cacheRepository = cacheRepository;
            _logger = logger;
            _client = BuildMoviesApiClient(apiClientOptions.Value);
        }

        public ApiClientGrpc(IOptions<ApiClientConfiguration> apiClientOptions, 
            IMapper mapper, 
            MoviesApi.MoviesApiClient client,
            ICacheRepository cacheRepository,
            ILogger<ApiClientGrpc> logger)
        {
            _mapper = mapper;
            _client = client;
            _cacheRepository = cacheRepository;
            _logger = logger;
        }

        public async Task<ShowResponse> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"Argument {nameof(id)} is incorrect.");
            }
            
            var request = new IdRequest()
            {
                Id = id
            };

            ShowResponse response = null;

            try
            {
                var grpcResponse = await _client.GetByIdAsync(request);
                if (grpcResponse.Success)
                {
                    grpcResponse.Data.TryUnpack<showResponse>(out var data);
                    response = _mapper.Map<ShowResponse>(data);
                    await _cacheRepository.SetValueAsync($"{Name}-get-by-id-{id}", response);
                }
                else
                {
                    _logger.LogError("Call GetByIdAsync with id {id} failed. Errors: {error}", id, grpcResponse.Exceptions.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Can not get data from grpc GetByIdAsync resource for id {id}. Error: {exception}", id, e);
                response = await _cacheRepository.GetValueAsync<ShowResponse>($"{Name}-get-by-id-{id}");
            }

            return response;
        }

        public async Task<ShowListResponse> SearchAsync(string text)
        {
            var request = new SearchRequest()
            {
                Text = text
            };

            ShowListResponse response = null;
            
            try
            {
                var grpcResponse = await _client.SearchAsync(request);
                if (grpcResponse.Success)
                {
                    grpcResponse.Data.TryUnpack<showListResponse>(out var data);
                    response = _mapper.Map<ShowListResponse>(data);
                    await _cacheRepository.SetValueAsync($"{Name}-search-{text}", response);
                }
                else
                {
                    _logger.LogError("Call SearchAsync with id {text} failed. Errors: {error}", text, grpcResponse.Exceptions.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Can not get data from grpc SearchAsync resource for id {text}. Error: {exception}", text, e);
                response = await _cacheRepository.GetValueAsync<ShowListResponse>($"{Name}-search-{text}");
            }
            
            if (response is null)
            {
                return new ShowListResponse
                {
                    ShowResponses = new List<ShowResponse>()
                };
            }

            return response;
        }

        public async Task<ShowListResponse> GetAllAsync()
        {
            ShowListResponse response = null;

            try
            {
                var grpcResponse = await _client.GetAllAsync(new Empty());
                if (grpcResponse.Success)
                {
                    grpcResponse.Data.TryUnpack<showListResponse>(out var data);
                    response = _mapper.Map<ShowListResponse>(data);
                    await _cacheRepository.SetValueAsync($"{Name}-get-all", response);
                }
                else
                {
                    _logger.LogError("Call GetAllAsync failed. Errors: {error}", grpcResponse.Exceptions.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Can not get data from grpc GetAllAsync resource. Error: {exception}", e);
                response = await _cacheRepository.GetValueAsync<ShowListResponse>($"{Name}-get-all");
            }

            if (response is null)
            {
                return new ShowListResponse
                {
                    ShowResponses = new List<ShowResponse>()
                };
            }

            return response;
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