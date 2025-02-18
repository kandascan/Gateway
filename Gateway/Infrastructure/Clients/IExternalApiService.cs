using Gateway.ApplicationCore.DTOs;
using Gateway.ApplicationCore.JsonPlaceholder;
using Refit;

namespace Gateway.Infrastructure.Clients
{
    [Headers("Accept: application/json")]
    public interface IExternalApi
    {
        [Get("/objects")]
        Task<ProductDto[]> GetProductsAsync();
    }

    [Headers("Accept: application/json")]
    public interface IJsonPlaceholder
    {
        [Get("/posts/{id}")]
        Task<PostDto> GetPostById(int id);

        [Get("/posts")]
        Task<PostDto[]> GetPosts();
    }
}
