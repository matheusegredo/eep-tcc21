using Microsoft.Extensions.Caching.Memory;
using System;

namespace Tcc.Text_to_Speech.Application
{
    public class Cache
    {
        private readonly IMemoryCache _cache;

        public Cache()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public (bool exist, TResult result) Read<TKey, TResult>(TKey key) 
            => (_cache.TryGetValue(key, out TResult result), result);

        public void Add<TKey>(TKey key, object value) 
            => _cache.Set(key, value, TimeSpan.FromSeconds(120));
    }
}
