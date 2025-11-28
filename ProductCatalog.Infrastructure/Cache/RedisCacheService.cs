using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ProductCatalog.Application.Interfaces;

namespace ProductCatalog.Infrastructure.Cache
{
    public class RedisCacheService(IDistributedCache cache) : ProductCatalog.Application.Interfaces.ICacheService
    {
        private readonly IDistributedCache _cache = cache;

        public async Task<T?> GetAsync<T>(string key)
        {
            var data = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(data)) return default;
            return JsonSerializer.Deserialize<T>(data);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
        {
            var options = new DistributedCacheEntryOptions();
            if (absoluteExpiration.HasValue)
                options.SetAbsoluteExpiration(absoluteExpiration.Value);
            var data = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, data, options);
        }

        public Task RemoveAsync(string key)
        {
            return _cache.RemoveAsync(key);
        }

        public Task<string?> GetStringAsync(string key)
        {
            return _cache.GetStringAsync(key);
        }

        public Task SetStringAsync(string key, string value, TimeSpan? absoluteExpiration = null)
        {
            var options = new DistributedCacheEntryOptions();
            if (absoluteExpiration.HasValue)
                options.SetAbsoluteExpiration(absoluteExpiration.Value);
            return _cache.SetStringAsync(key, value, options);
        }
    }
}
