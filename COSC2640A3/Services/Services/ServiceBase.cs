using System;
using System.Linq;
using System.Threading.Tasks;
using COSC2640A3.DbContexts;
using COSC2640A3.Models;
using Helper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace COSC2640A3.Services.Services {

    public class ServiceBase {

        protected readonly MainDbContext _dbContext;

        private const int DefaultCacheExpirationMinutes = 5;

        protected ServiceBase(MainDbContext dbContext) {
            _dbContext = dbContext;
        }

        /// <summary>
        /// To save cache. Possible combinations of keys in DataCache:
        /// (DataType, DataId, DataKey); (DataType, DataId); (DataType, DataKey); DataKey; SearchInput;
        /// </summary>
        /// <param name="cache"></param>
        /// <typeparam></typeparam>
        /// <returns type="bool"></returns>
        protected async Task<bool> SaveCache(DataCache cache) {
            if (cache is null) throw new ArgumentNullException();

            var entry = await GetCacheEntry(cache);
            if (entry is not null && entry.Timestamp + DefaultCacheExpirationMinutes * 60 * 1000 >= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) return true;
            if (entry is not null && entry.Timestamp + DefaultCacheExpirationMinutes * 60 * 1000 < DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() && !await RemoveCacheEntry(cache)) return false;

            await _dbContext.DataCaches.AddAsync(cache);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException) { return false; }
        }

        /// <summary>
        /// To get cache. Possible combinations of keys in DataCache:
        /// (DataType, DataId, DataKey); (DataType, DataId); (DataType, DataKey); (DataType, SearchInput); DataKey;
        /// </summary>
        /// <param name="cacheInfo"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns type="T?"></returns>
        protected async Task<T> GetCache<T>(DataCache cacheInfo) {
            try {
                var matchedEntry = await GetCacheEntry(cacheInfo);
                if (matchedEntry is null) return default;

                var isExpired = matchedEntry.Timestamp + DefaultCacheExpirationMinutes * 60 * 1000 < DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (!isExpired) return JsonConvert.DeserializeObject<T>(matchedEntry.SerializedData);
                
                _ = await RemoveCacheEntry(matchedEntry);
                return default;
            }
            catch (ArgumentNullException) { return default; }
        }

        private async Task<DataCache> GetCacheEntry(DataCache cache) {
            try {
                DataCache entry = null;
                
                if (Helpers.IsProperString(cache.DataType) && Helpers.IsProperString(cache.DataId) && Helpers.IsProperString(cache.DataKey))
                    entry = await _dbContext.DataCaches
                                            .Where(c =>
                                                c.DataType.Equals(cache.DataType) &&
                                                c.DataId.Equals(cache.DataId) &&
                                                c.DataKey.Equals(cache.DataKey)
                                            )
                                            .FirstOrDefaultAsync();
                else if (Helpers.IsProperString(cache.DataType) && Helpers.IsProperString(cache.DataId))
                    entry = await _dbContext.DataCaches
                                            .Where(c =>
                                                c.DataType.Equals(cache.DataType) &&
                                                c.DataId.Equals(cache.DataId)
                                            )
                                            .FirstOrDefaultAsync();
                else if (Helpers.IsProperString(cache.DataType) && Helpers.IsProperString(cache.DataKey))
                    entry = await _dbContext.DataCaches
                                            .Where(c =>
                                                c.DataType.Equals(cache.DataType) &&
                                                c.DataKey.Equals(cache.DataKey)
                                            )
                                            .FirstOrDefaultAsync();
                else if (Helpers.IsProperString(cache.DataType) && Helpers.IsProperString(cache.SearchInput))
                    entry = await _dbContext.DataCaches
                                            .Where(c =>
                                                c.DataType.Equals(cache.DataType) &&
                                                c.SearchInput.Equals(cache.SearchInput)
                                            )
                                            .FirstOrDefaultAsync();
                else if (Helpers.IsProperString(cache.DataKey))
                    entry = await _dbContext.DataCaches.Where(c => c.DataKey.Equals(cache.DataKey)).FirstOrDefaultAsync();

                return entry;
            }
            catch (ArgumentNullException) { return default; }
        }

        private async Task<bool> RemoveCacheEntry(DataCache cache) {
            _dbContext.DataCaches.Remove(cache);

            try {
                var result = await _dbContext.SaveChangesAsync();
                return result != 0;
            }
            catch (DbUpdateException) { return false; }
        }
    }
}