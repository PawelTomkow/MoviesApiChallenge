using System;
using System.Text.Json;
using System.Threading.Tasks;
using ApiApplication.Core.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Clients.Cache
{
    public class InMemoryCacheRepository : ICacheRepository
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<InMemoryCacheRepository> _logger;

        public InMemoryCacheRepository(IMemoryCache memoryCache, ILogger<InMemoryCacheRepository> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }
        
        public Task SetValueAsync(string key, object value)
        {
            var serializedObject = JsonSerializer.Serialize(@value);
            var savedObject = _memoryCache.Set(key, serializedObject);

            if (savedObject is null)
            {
                _logger.LogError("Issue with saving: Key {key}, Value: {value}.", key, value);
                throw new CacheException($"Issue with saving: Key {key}, Value: {value}");
            }

            return Task.CompletedTask;
        }

        public Task SetValueWithExpiryAsync(string key, object value, TimeSpan expiry)
        {
            var serializedObject = JsonSerializer.Serialize(@value);
            var savedObject = _memoryCache.Set(key, serializedObject, expiry);

            if (savedObject is null)
            {
                _logger.LogError("Issue with saving: Key {key}, Value: {value}.", key, value);
                throw new CacheException($"Issue with saving: Key {key}, Value: {value}");
            }

            return Task.CompletedTask;
        }

        public Task<T> GetValueAsync<T>(string key)
        {
            var response = default(T);
            try
            {
                _memoryCache.TryGetValue(key, out string retrievedObject);
                if (!string.IsNullOrEmpty(retrievedObject))
                {
                    response =  JsonSerializer.Deserialize<T>(retrievedObject);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Can not get value from redis. Error: {error}", e.Message);
            }

            return Task.FromResult(response);
        }
    }
}