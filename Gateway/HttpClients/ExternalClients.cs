using Gateway.Infrastructure.Clients;
using Refit;

namespace Gateway.HttpClients
{
    public static class ExternalClients
    {
        public static IServiceCollection AddExternalClients(this IServiceCollection services)
        {
            services.AddRefitClient<IExternalApi>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.restful-api.dev"));

            services.AddRefitClient<IJsonPlaceholder>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://jsonplaceholder.typicode.com"));

            return services;
        }
    }
}
