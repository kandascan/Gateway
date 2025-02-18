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

        public object? GetFromCache(Guid requestId, string type)
        {
            if (_cacheHandlers.TryGetValue(type, out var handler))
            {
                return handler.GetData(requestId);
            }

            return default;
        }
    }
}