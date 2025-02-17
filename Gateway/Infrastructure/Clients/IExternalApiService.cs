using Gateway.ApplicationCore.DTOs;
using Refit;

namespace Gateway.Infrastructure.Clients
{
    [Headers("Accept: application/json")]
    public interface IExternalApi
    {
        [Get("/objects")]
        Task<ProductDto[]> GetProductsAsync();
    }
}
