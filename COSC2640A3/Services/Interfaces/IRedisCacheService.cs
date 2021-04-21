using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using COSC2640A3.Bindings;

namespace COSC2640A3.Services.Interfaces {

    public interface IRedisCacheService {
        
        Task InsertRedisCacheEntry([NotNull] CacheEntry entry);

        Task<T> GetRedisCacheEntry<T>([NotNull] string entryKey);
    }
}