using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductCatalog.Tests.TestDoubles
{
    public class InMemoryCacheService : ProductCatalog.Application.Interfaces.ICacheService
    {
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> _store = new();

        public Task<T?> GetAsync<T>(string key)
        {
            if (_store.TryGetValue(key, out var val))
            {
                var obj = System.Text.Json.JsonSerializer.Deserialize<T>(val);
                return Task.FromResult(obj);
            }
            return Task.FromResult<T?>(default);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
        {
            var val = System.Text.Json.JsonSerializer.Serialize(value);
            _store[key] = val;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _store.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        public Task<string?> GetStringAsync(string key)
        {
            _store.TryGetValue(key, out var val);
            return Task.FromResult<string?>(val);
        }

        public Task SetStringAsync(string key, string value, TimeSpan? absoluteExpiration = null)
        {
            _store[key] = value;
            return Task.CompletedTask;
        }
    }
}
