namespace Gateway.CacheHandlers
{
    public interface ICacheHandler
    {
        Task<object?> GetDataAsync(Guid requestId);
        string Type { get; } // Jawnie określony typ

    }

}
