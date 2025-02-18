namespace Gateway.CacheHandlers
{
    public interface ICacheHandler
    {
        object? GetData(Guid requestId);
        string Type { get; } // Jawnie określony typ

    }

}
