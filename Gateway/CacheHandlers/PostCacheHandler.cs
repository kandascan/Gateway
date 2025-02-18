using Gateway.ApplicationCore.JsonPlaceholder;

namespace Gateway.CacheHandlers
{
    public class PostCacheHandler : ICacheHandler
    {
        private readonly ApiRequestQueue _queue;
        public string Type => "post"; // Jawnie określony typ

        public PostCacheHandler(ApiRequestQueue queue)
        {
            _queue = queue;
        }

        public async Task<object?> GetDataAsync(Guid requestId)
        {
            var (found, data) = await _queue.TryGetResultAsync<PostDto>(requestId);
            return found ? new { requestId, data } : default;
        }
    }

}
