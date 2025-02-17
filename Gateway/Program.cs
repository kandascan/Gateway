using AutoMapper;
using Gateway.ApplicationCore.DTOs;
using Gateway.ApplicationCore.Models;
using Gateway.BackgroundWorker;
using Gateway.Cache;
using Gateway.Config;
using Gateway.Http;
using Gateway.HttpClients;
using Gateway.Infrastructure.AutoMapperProfiles;
using Gateway.Infrastructure.Clients;
using Gateway.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilogLogging(builder.Configuration);

// Pobranie connection string z konfiguracji
var redisConfig = builder.Configuration.GetSection("Redis");
var redisConnectionString = redisConfig.GetValue<string>("ConnectionString") ?? "localhost:6379";
//var defaultCacheExpiration = redisConfig.GetValue<int>("DefaultCacheExpiration", 30); // Domyślnie 30 minut
// Rejestracja Redis jako Singleton
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.Configure<RedisSettings>(redisConfig);
builder.Services.AddSingleton<RedisCacheService>();

builder.Services.AddAutoMapper(typeof(ExternalDataProfile));
builder.Services.AddSingleton<ApiRequestQueue>();
builder.Services.AddHostedService<QueueWorker>();

builder.Services.AddExternalClients();
builder.Services.AddHttpPolicy();

builder.Services.AddHealthChecks()
    .AddRedis(redisConnectionString, name: "redis");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});


//Wrzucenie zadania do kolejki
app.MapGet("/setProductRequestToQueue", async (ApiRequestQueue queue, IMapper mapper, ILogger<Program> logger, IExternalApi service) =>
{
    var requestId = queue.Enqueue(async () => {
        var externalData = await service.GetProductsAsync();
        Random rand = new Random();
        int randomValue = rand.Next(1, externalData.Count());
        ProductDto produkt = externalData[randomValue];
        return produkt;
    });

    return Results.Ok(new { requestId });
})
            .WithName("setProductRequestToQueue")
            .WithOpenApi();

//Pobranie odpowiedzi z kolejki
app.MapGet("/getResponsFromCache", (Guid requestId, ApiRequestQueue queue, IMapper mapper, ILogger<Program> logger, IExternalApi service) =>
{
    if (/*type == "products" && */queue.TryGetResult<ProductDto>(requestId, out var products))
    {
        return Results.Ok(new { requestId, data = products });
    }
    //else if (type == "users" && _queue.TryGetResult<UserDto[]>(requestId, out var users))
    //{
    //    return Results.Ok(new { requestId, data = users });
    //}

    return Results.NotFound(new { message = "Dane jeszcze nie gotowe lub nie istnieją." });
})
            .WithName("getResponsFromCache")
            .WithOpenApi();


//Testowe endpointy do sprawdzania zewnetrzego api bez kolejki
app.MapGet("/getproductswithoutqueue", async (ApiRequestQueue queue, IMapper mapper, ILogger<Program> logger, IExternalApi service) =>
{
    var externalData = await service.GetProductsAsync();
    logger.LogInformation("Pobrano dane z zewnętrznego serwisu");
    

    return Results.Ok(externalData);
})
            .WithName("GetProductsWithoutQueue")
            .WithOpenApi();



//Testowe endpointy do obsługi cache
app.MapGet("/setproducttoredis", async (RedisCacheService cacheService, IMapper mapper, ILogger<Program> logger, IExternalApi service) =>
{
    var externalData = await service.GetProductsAsync();
    Produkt[] mappedData = mapper.Map<Produkt[]>(externalData);

    Guid requestId = Guid.NewGuid();

    Random rand = new Random();
    int randomValue = rand.Next(1, mappedData.Count());
    Produkt produkt = mappedData[randomValue];

    await cacheService.SetValueAsync(requestId, produkt);
    logger.LogInformation($"Zapisano w Redisie pod kluczem: {requestId}");

    return Results.Ok(new {requestId, produkt});
})
.WithName("setproductstoredis")
.WithOpenApi();

app.MapGet("/getproductfromredis", async (Guid requestId, RedisCacheService cacheService, ILogger<Program> logger) =>
{
    Produkt? produkt = await cacheService.GetValueAsync<Produkt>(requestId);
    
    logger.LogInformation($"Pobrano obiekt z redisa o kluczu: {requestId}");
    return Results.Ok(new { requestId, produkt, ResponseTime = DateTime.Now.ToShortTimeString() });
})
.WithName("getproductfromredis")
.WithOpenApi();

app.MapGet("/getcachestatus", (RedisCacheService cacheService, ILogger<Program> logger) =>
{
    var endpoint = cacheService.GetStatus();
    return Results.Ok(new { Status = "OK", Endpoint = endpoint });
})
.WithName("getcachestatus")
.WithOpenApi();


app.MapDelete("/delproductfromredis", async (Guid requestId, RedisCacheService cacheService, ILogger<Program> logger) =>
{
    bool isDeleted = await cacheService.DeleteAsync(requestId);

    logger.LogInformation($"Usunięto obiekt z redisa o kluczu: {requestId}");
    return Results.Ok(new { isDeleted });
})
.WithName("delproductfromredis")
.WithOpenApi();

app.Run();