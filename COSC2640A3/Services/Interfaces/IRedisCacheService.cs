using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using COSC2640A3.Bindings;

namespace COSC2640A3.Services.Interfaces {

    public interface IRedisCacheService {
        
        /// <summary>
        /// Void return. The cache entry must not be null.
        /// </summary>
        Task InsertRedisCacheEntry([NotNull] CacheEntry entry);

        /// <summary>
        /// Generic method. Works with any object type T. The entry key must not be null.
        /// </summary>
        Task<T> GetRedisCacheEntry<T>([NotNull] string entryKey);
        
        /// <summary>
        /// Void return. The cache entry must not be null.
        /// </summary>
        Task RemoveCacheEntry([NotNull] string entryKey);
    }
}