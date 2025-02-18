using Gateway.CacheHandlers;

namespace Gateway.Cache
{
    public class CacheService
    {
        private readonly Dictionary<string, ICacheHandler> _cacheHandlers;

        public CacheService(IEnumerable<ICacheHandler> handlers)
        {
            _cacheHandlers = handlers.ToDictionary(h => h.Type, h => h);
        }

        public async Task<object?> GetFromCacheAsync(Guid requestId, string type)
        {
            if (_cacheHandlers.TryGetValue(type, out var handler))
            {
                return await handler.GetDataAsync(requestId);
            }

            return default;
        }
    }
}