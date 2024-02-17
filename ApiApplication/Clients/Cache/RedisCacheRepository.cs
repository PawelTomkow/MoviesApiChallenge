using System;
using System.Text.Json;
using System.Threading.Tasks;
using ApiApplication.Configurations;
using ApiApplication.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ApiApplication.Clients.Cache
{
    public class RedisCacheRepository : ICacheRepository
    {
        private readonly ILogger<RedisCacheRepository> _logger;
        private readonly IDatabase _redis;

        public RedisCacheRepository(IOptions<CacheConfiguration> configuration, ILogger<RedisCacheRepository> logger)
        {
            if (string.IsNullOrWhiteSpace(configuration?.Value.ConfigurationString))
            { 
                throw new CacheException("Can not create RedisCacheRepository. ConfigurationString is null."); 
            }

            _logger = logger;

            var connection = ConnectionMultiplexer.Connect(configuration?.Value.ConfigurationString);
            _redis = connection.GetDatabase();

        }
        
        public async Task SetValueAsync(string key, object value)
        {
            var serializedObject = JsonSerializer.Serialize(@value);
            var isSaved = await _redis.StringSetAsync(key, serializedObject);
            
            if (!isSaved)
            {
                _logger.LogError("Issue with saving: Key {key}, Value: {value}.", key, value);
                throw new CacheException($"Issue with saving: Key {key}, Value: {value}");
            }
        }

        public async Task SetValueWithExpiryAsync(string key, object value, TimeSpan expiry)
        {
            var serializedObject = JsonSerializer.Serialize(@value);
            var isSaved = await _redis.StringSetAsync(key, serializedObject, expiry);

            if (!isSaved)
            {
                _logger.LogError("Issue with saving: Key {key}, Value: {value}.", key, value);
                throw new CacheException($"Issue with saving: Key {key}, Value: {value}");
            }
        }

        public async Task<T> GetValueAsync<T>(string key)
        {
            var response = default(T);
            try
            {
                var retrievedObject = await _redis.StringGetAsync(key);
                if (!retrievedObject.IsNullOrEmpty)
                {
                    response =  JsonSerializer.Deserialize<T>(retrievedObject);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Can not get value from redis. Error: {error}", e.Message);
            }

            return response;
        }
    }
}