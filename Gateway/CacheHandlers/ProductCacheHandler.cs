using AutoMapper;
using Gateway.ApplicationCore.DTOs;
using Gateway.ApplicationCore.Models;
using Gateway.Enums;

namespace Gateway.CacheHandlers
{
    public class ProductCacheHandler : ICacheHandler
    {
        private readonly ApiRequestQueue _queue;
        private readonly IMapper _mapper;

        public RequestTypeEnum Type => RequestTypeEnum.Product; // Jawnie określony typ

        public ProductCacheHandler(ApiRequestQueue queue, IMapper mapper)
        {
            _queue = queue;
            _mapper = mapper;
        }

        public async Task<object?> GetDataAsync(Guid requestId)
        {
            var (found, data) = await _queue.TryGetResultAsync<ProductDto>(requestId);
            if (found)
            {
                var mappedData = _mapper.Map<Produkt>(data);
                return new { requestId, data = mappedData };
            }
            return default;
        }
    }

}
