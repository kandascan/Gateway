using Gateway.Enums;

namespace Gateway.CacheHandlers
{
    public interface ICacheHandler
    {
        Task<object?> GetDataAsync(Guid requestId);
        RequestTypeEnum Type { get; } // Jawnie określony typ

    }

}
