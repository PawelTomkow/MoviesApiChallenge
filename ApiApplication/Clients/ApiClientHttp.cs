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
            }
            catch (FlurlHttpTimeoutException exception)
            {
                _logger.LogError(
                    "Timeout exception.Call endpoint {endpoint} with id {id} failed. Errors: {error}",$"{EndpointName}/{id}", id, exception.Message);
                throw new ResourceUnavailableException(
                    $"Unknown error with access to resource {EndpointName}/id with id: {id}");
            }
            catch (FlurlHttpException exception)
            {
                switch (exception.StatusCode)
                {
                    case (int)HttpStatusCode.Unauthorized:
                    case (int)HttpStatusCode.NotFound:
                        var errorResponse = await exception.GetResponseJsonAsync<ErrorResponse>();
                        _logger.LogError("Call endpoint {endpoint} with id {id} failed. Errors: {error}",$"{EndpointName}/{id}", id, errorResponse.Message);
                        throw new ResourceUnavailableException($"Can not access to resource with id: {id}");
                    default:
                        _logger.LogError("Call endpoint {endpoint} with id {id} failed. Errors: {error}",$"{EndpointName}/{id}", id, exception.Message);
                        throw new ResourceUnavailableException(
                            $"Unknown error with access to resource {EndpointName}/id with id: {id}");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    "Unknown exception. Call endpoint {endpoint} with id {id} failed. Errors: {error}",$"{EndpointName}/{id}", id, exception.Message);
                throw new ResourceUnavailableException($"Unknown error with access to resource {EndpointName}/id with id: {id}");
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
            }
            catch (FlurlHttpTimeoutException exception)
            {
                _logger.LogError(
                    "Timeout exception. Call endpoint {endpoint} with search query: {text} failed. Errors: {error}",
                    $"{EndpointName}?search={text}", text, exception.Message);
                throw new ResourceUnavailableException($"Unknown error with access to resource search query: {text}");
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
                        throw new ResourceUnavailableException($"Can not access to resource with search query: {text}");
                    default:
                        _logger.LogError("Call endpoint {endpoint} with search query: {text} failed. Errors: {error}",
                            $"{EndpointName}?search={text}", text, exception.Message);
                        throw new ResourceUnavailableException(
                            $"Unknown error with access to resource search query: {text}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(
                    "Unknown exception. Call endpoint {endpoint} with search query: {text} failed. Errors: {error}",
                    $"{EndpointName}?search={text}", text, e.Message);
                throw new ResourceUnavailableException($"Unknown error with access to resource search query: {text}");
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
            }
            catch (FlurlHttpTimeoutException exception)
            {
                _logger.LogError(
                    "Timeout exception. Call endpoint {endpoint}. Errors: {error}",EndpointName, exception.Message);
                throw new ResourceUnavailableException($"Unknown error with access to resource.");
            }
            catch (FlurlHttpException exception)
            {
                switch (exception.StatusCode)
                {
                    case (int)HttpStatusCode.Unauthorized:
                    case (int)HttpStatusCode.NotFound:
                        var errorResponse = await exception.GetResponseJsonAsync<ErrorResponse>();
                        _logger.LogError("Call endpoint {endpoint}. Errors: {error}",EndpointName, errorResponse.Message);
                        throw new ResourceUnavailableException($"Can not access to resource.");
                    default:
                        _logger.LogError("Call endpoint {endpoint}. Errors: {error}",EndpointName, exception.Message);
                        throw new ResourceUnavailableException(
                            $"Unknown error with access to resource.");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    "Unknown error. Call endpoint {endpoint}. Errors: {error}",EndpointName, exception.Message);
                throw new ResourceUnavailableException($"Unknown error with access to resource.");
            }

            return response;
        }
    }
}