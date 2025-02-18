using AutoMapper;
using Gateway.ApplicationCore.DTOs;
using Gateway.ApplicationCore.Models;

namespace Gateway.CacheHandlers
{
    public class ProductCacheHandler : ICacheHandler
    {
        private readonly ApiRequestQueue _queue;
        private readonly IMapper _mapper;

        public string Type => "product"; // Jawnie określony typ

        public ProductCacheHandler(ApiRequestQueue queue, IMapper mapper)
        {
            _queue = queue;
            _mapper = mapper;
        }

        public object? GetData(Guid requestId)
        {
            if (_queue.TryGetResult<ProductDto>(requestId, out var data))
            {
                var mappedData = _mapper.Map<Produkt>(data);

                return new { requestId, data = mappedData };
            }

            return default;
        }
    }

}
