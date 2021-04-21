using System;
using System.Threading.Tasks;
using COSC2640A3.Bindings;
using COSC2640A3.Services.Interfaces;
using Helper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace COSC2640A3.Services.Services {

    public sealed class RedisCacheService : IRedisCacheService {
        
        private class CacheSettings {
            public bool IsEnabled { get; set; }
            public int SlidingExpiration { get; set; }
            public int AbsoluteExpiration { get; set; }
        }

        private readonly IDistributedCache _redisCache;
        private readonly CacheSettings _cacheSettings = new();

        public RedisCacheService(
            IDistributedCache redisCache,
            IOptions<COSC2640A3Options> options
        ) {
            _redisCache = redisCache;

            _cacheSettings.IsEnabled = bool.Parse(options.Value.CacheEnabled);
            _cacheSettings.SlidingExpiration = int.Parse(options.Value.CacheSlidingExpiration);
            _cacheSettings.AbsoluteExpiration = int.Parse(options.Value.CacheAbsoluteExpiration);
        }

        public async Task InsertRedisCacheEntry(CacheEntry entry) {
            if (!_cacheSettings.IsEnabled) return;

            await _redisCache.SetAsync(
                $"{ entry.EntryKey }",
                entry.Data.EncodeDataUtf8(),
                new DistributedCacheEntryOptions {
                    SlidingExpiration = TimeSpan.FromDays(_cacheSettings.SlidingExpiration),
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(_cacheSettings.AbsoluteExpiration)
                }
            );
        }

        public async Task<T> GetRedisCacheEntry<T>(string entryKey) {
            if (!_cacheSettings.IsEnabled) return default;

            try {
                var cachedData = await _redisCache.GetAsync(entryKey);
                return cachedData.Length == 0 ? default : cachedData.DecodeUtf8<T>();
            }
            catch (Exception) {
                return default;
            }
        }
    }
}