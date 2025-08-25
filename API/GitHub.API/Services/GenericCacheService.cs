using Microsoft.Extensions.Caching.Memory;

namespace GitHub.API.Services
{
    public interface IGenericCacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? duration = null);
        void Remove(string key);
    }
    public class GenericCacheService(IMemoryCache cache) : IGenericCacheService
    {
        private readonly IMemoryCache _cache = cache;
        private readonly TimeSpan _defaultDuration = TimeSpan.FromHours(12);
        public T? Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out T value))
                return value;
            return default;
        }

        public void Set<T>(string key, T value, TimeSpan? duration = null)
        {
            _cache.Set(key, value, duration ?? _defaultDuration);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

    }
}
