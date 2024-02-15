using System;
using System.Threading.Tasks;

namespace ApiApplication.Clients.Cache
{
    public interface ICacheRepository
    {
        Task SetValueAsync(string key, object value);
        Task SetValueWithExpiryAsync(string key, object value, TimeSpan expiry);

        Task<T> GetValueAsync<T>(string key);
    }
}