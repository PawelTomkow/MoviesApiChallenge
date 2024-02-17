using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ApiApplication.Clients.Cache;
using ApiApplication.Clients.Contracts;
using ApiApplication.Configurations;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Core.Exceptions;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApiApplication.Clients
{
    public class ApiClientHttp : IApiClient
    {
        private readonly ILogger<ApiClientHttp> _logger;
        private readonly ICacheRepository _cacheRepository;
        private readonly ApiClientConfiguration _clientConfiguration;
        private const string EndpointName = "v1/movies";
        private const string Name = "ApiClient";

        public ApiClientHttp(ILogger<ApiClientHttp> logger, ICacheRepository cacheRepository, IOptions<ApiClientConfiguration> options)
        {
            _clientConfiguration = options.Value;
            
            if (_clientConfiguration is null)
            {
                throw new ConfigurationException("Can not load ApiClientConfiguration.");
            }

            if (string.IsNullOrWhiteSpace(_clientConfiguration.BaseAddress))
            {
                throw new ConfigurationException($"BaseAddress can not be null or empty");
            }
            
            _logger = logger;
            _cacheRepository = cacheRepository;
        }
        
        public async Task<ShowResponse> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException($"Argument {nameof(id)} is incorrect.");
            }

            ShowResponse response = null;

            try
            {
                response = await _clientConfiguration.BaseAddress
                    .AppendPathSegment(EndpointName)
                    .WithHeader("X-Apikey", _clientConfiguration.ApiKey)
                    .AppendPathSegment(id)
                    .GetJsonAsync<ShowResponse>();
                
                await _cacheRepository.SetValueAsync($"{Name}-get-all", response);
            }
            catch (FlurlHttpTimeoutException exception)
            {
                _logger.LogError(
                    "Timeout exception.Call endpoint {endpoint} with id {id} failed. Errors: {error}",$"{EndpointName}/{id}", id, exception.Message);
                response = await _cacheRepository.GetValueAsync<ShowResponse>($"{Name}-get-by-id-{id}");
            }
            catch (FlurlHttpException exception)
            {
                switch (exception.StatusCode)
                {
                    case (int)HttpStatusCode.Unauthorized:
                    case (int)HttpStatusCode.NotFound:
                        var errorResponse = await exception.GetResponseJsonAsync<ErrorResponse>();
                        _logger.LogError("Call endpoint {endpoint} with id {id} failed. Errors: {error}",$"{EndpointName}/{id}", id, errorResponse.Message);
                        response = await _cacheRepository.GetValueAsync<ShowResponse>($"{Name}-get-by-id-{id}");
                        break;
                    default:
                        _logger.LogError("Call endpoint {endpoint} with id {id} failed. Errors: {error}",$"{EndpointName}/{id}", id, exception.Message);
                        response = await _cacheRepository.GetValueAsync<ShowResponse>($"{Name}-get-by-id-{id}");
                        break;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    "Unknown exception. Call endpoint {endpoint} with id {id} failed. Errors: {error}",$"{EndpointName}/{id}", id, exception.Message);
                response = await _cacheRepository.GetValueAsync<ShowResponse>($"{Name}-get-by-id-{id}");
            }

            return response;
        }

        public async Task<ShowListResponse> SearchAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException($"Argument {nameof(text)} is incorrect.");
            }
            
            ShowListResponse response = new ShowListResponse();

            try
            {
                response.ShowResponses = await _clientConfiguration.BaseAddress
                    .AppendPathSegment(EndpointName)
                    .WithHeader("X-Apikey", _clientConfiguration.ApiKey)
                    .AppendQueryParam("search", text)
                    .GetJsonAsync<List<ShowResponse>>();
                
                await _cacheRepository.SetValueAsync($"{Name}-search-{text}", response);
            }
            catch (FlurlHttpTimeoutException exception)
            {
                _logger.LogError(
                    "Timeout exception. Call endpoint {endpoint} with search query: {text} failed. Errors: {error}",
                    $"{EndpointName}?search={text}", text, exception.Message);
                response = await _cacheRepository.GetValueAsync<ShowListResponse>($"{Name}-search-{text}");
            }
            catch (FlurlHttpException exception)
            {
                switch (exception.StatusCode)
                {
                    case (int)HttpStatusCode.Unauthorized:
                    case (int)HttpStatusCode.NotFound:
                        var errorResponse = await exception.GetResponseJsonAsync<ErrorResponse>();
                        _logger.LogError("Call endpoint {endpoint} with search query: {text} failed. Errors: {error}",
                            $"{EndpointName}?search={text}", text, errorResponse.Message);
                        response = await _cacheRepository.GetValueAsync<ShowListResponse>($"{Name}-search-{text}");
                        break;
                    default:
                        _logger.LogError("Call endpoint {endpoint} with search query: {text} failed. Errors: {error}",
                            $"{EndpointName}?search={text}", text, exception.Message);
                        response = await _cacheRepository.GetValueAsync<ShowListResponse>($"{Name}-search-{text}");
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "Unknown exception. Call endpoint {endpoint} with search query: {text} failed. Errors: {error}",
                    $"{EndpointName}?search={text}", text, e.Message);
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
            ShowListResponse response = new ShowListResponse();
            try
            {
                response.ShowResponses = await _clientConfiguration.BaseAddress
                    .AppendPathSegment(EndpointName)
                    .WithHeader("X-Apikey", _clientConfiguration.ApiKey)
                    .GetJsonAsync<List<ShowResponse>>();
                
                await _cacheRepository.SetValueAsync($"{Name}-get-all", response);
            }
            catch (FlurlHttpTimeoutException exception)
            {
                _logger.LogError(
                    "Timeout exception. Call endpoint {endpoint}. Errors: {error}",EndpointName, exception.Message);
                response = await _cacheRepository.GetValueAsync<ShowListResponse>($"{Name}-get-all");
            }
            catch (FlurlHttpException exception)
            {
                switch (exception.StatusCode)
                {
                    case (int)HttpStatusCode.Unauthorized:
                    case (int)HttpStatusCode.NotFound:
                        var errorResponse = await exception.GetResponseJsonAsync<ErrorResponse>();
                        _logger.LogError("Call endpoint {endpoint}. Errors: {error}",EndpointName, errorResponse.Message);
                        response = await _cacheRepository.GetValueAsync<ShowListResponse>($"{Name}-get-all");
                        break;
                    default:
                        _logger.LogError("Call endpoint {endpoint}. Errors: {error}",EndpointName, exception.Message);
                        response = await _cacheRepository.GetValueAsync<ShowListResponse>($"{Name}-get-all");
                        break;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    "Unknown error. Call endpoint {endpoint}. Errors: {error}",EndpointName, exception.Message);
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
    }
}