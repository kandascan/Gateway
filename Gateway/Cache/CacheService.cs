using Gateway.CacheHandlers;
using Gateway.Enums;

namespace Gateway.Cache
{
    public class CacheService
    {
        private readonly Dictionary<RequestTypeEnum, ICacheHandler> _cacheHandlers;

        public CacheService(IEnumerable<ICacheHandler> handlers)
        {
            _cacheHandlers = handlers.ToDictionary(h => h.Type, h => h);
        }

        public async Task<object?> GetFromCacheAsync(Guid requestId, RequestTypeEnum type)
        {
            if (_cacheHandlers.TryGetValue(type, out var handler))
            {
                return await handler.GetDataAsync(requestId);
            }

            return default;
        }
    }
}