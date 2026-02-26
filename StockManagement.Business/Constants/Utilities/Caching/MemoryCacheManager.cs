using Microsoft.Extensions.Caching.Memory;
using StockManagement.Business.Constants.Services;

namespace StockManagement.Business.Constants.Utilities.Caching
{
    public class MemoryCacheManager:ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T Get<T>(string key) => _memoryCache.Get<T>(key);

        public void Set<T>(string key, T value, TimeSpan expirationTime)
        {
            _memoryCache.Set(key, value, expirationTime);
        }

        public void Remove(string key) => _memoryCache.Remove(key);

        public bool Any(string key) => _memoryCache.TryGetValue(key, out _);
    }
}
