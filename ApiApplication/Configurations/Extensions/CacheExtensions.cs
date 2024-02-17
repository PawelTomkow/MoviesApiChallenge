using ApiApplication.Clients;
using ApiApplication.Clients.Cache;
using ApiApplication.Core.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiApplication.Configurations.Extensions
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            var apiClientConfiguration = new CacheConfiguration();
            configuration.GetSection(CacheConfiguration.Name).Bind(apiClientConfiguration);

            if (apiClientConfiguration is null)
            {
                throw new ConfigurationException($"Can not read {nameof(CacheConfiguration)} from settings.");
            }

            switch (apiClientConfiguration.Type)
            {
                case CacheTypes.Redis:
                    services.AddScoped<ICacheRepository, RedisCacheRepository>();
                    break;
                case CacheTypes.InMemory:
                    services.AddMemoryCache();
                    services.AddScoped<ICacheRepository, InMemoryCacheRepository>();
                    break;
                
                default:
                    throw new ConfigurationException($"Unknown type of Cache. Can not register {nameof(ICacheRepository)}");
            }

            return services;
        }
    }
}