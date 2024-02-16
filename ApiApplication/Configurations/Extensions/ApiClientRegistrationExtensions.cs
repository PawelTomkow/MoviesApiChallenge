using ApiApplication.Clients;
using ApiApplication.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiApplication.Configurations.Extensions
{
    public static class ApiClientRegistrationExtensions
    {
        public static IServiceCollection AddApiClient(this IServiceCollection services, IConfiguration configuration)
        {
            var apiClientConfiguration = new ApiClientConfiguration();
            configuration.GetSection(ApiClientConfiguration.Name).Bind(apiClientConfiguration);

            if (apiClientConfiguration is null)
            {
                throw new ConfigurationException($"Can not read {nameof(ApiClientConfiguration)} from settings.");
            }

            switch (apiClientConfiguration.Type)
            {
                case ApiClientTypes.Grpc:
                    services.AddScoped<IApiClient, ApiClientGrpc>();
                    break;
                case ApiClientTypes.Http:
                    services.AddScoped<IApiClient, ApiClientHttp>();
                    break;
                
                default:
                    throw new ConfigurationException("Unknown type of ApiClient. Can not register IApiClient");
            }

            return services;
        }
    }
}