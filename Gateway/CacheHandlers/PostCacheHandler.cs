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

        public object? GetData(Guid requestId)
        {
            if (_queue.TryGetResult<PostDto>(requestId, out var data))
            {
                // Możesz dodać mapowanie np. do `PostViewModel`
                return new { requestId, data };
            }

            return default;
        }
    }

}
