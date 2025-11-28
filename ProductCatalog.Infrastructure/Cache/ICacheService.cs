using System.Threading.Tasks;

namespace ProductCatalog.Infrastructure.Cache
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null);
        Task RemoveAsync(string key);
        Task<string?> GetStringAsync(string key);
        Task SetStringAsync(string key, string value, TimeSpan? absoluteExpiration = null);
    }
}
