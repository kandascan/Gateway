using Gateway.Config;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace Gateway.Cache
{
    public class RedisCacheService
    {
        private readonly IDatabase _database;
        private readonly TimeSpan _defaultExpiry;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IConnectionMultiplexer redis, IOptions<RedisSettings> options, ILogger<RedisCacheService> logger)
        {
            _database = redis.GetDatabase();
            _defaultExpiry = TimeSpan.FromMinutes(options.Value.DefaultCacheExpiration);
            _logger = logger;
        }

        public async Task<bool> SetValueAsync<T>(Guid key, T value)
        {
            try
            {
                string json = JsonSerializer.Serialize(value);
                await _database.StringSetAsync(key.ToString(), json, _defaultExpiry);
                _logger.LogInformation($"Set cache for key: {key}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache");

                return false;
            }
        }

        public async Task<T?> GetValueAsync<T>(Guid key)
        {
            try
            {
                string? json = await _database.StringGetAsync(key.ToString());
                return json is not null ? JsonSerializer.Deserialize<T>(json) : default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache");
                return default;
            }
        }

        public async Task<bool> DeleteAsync(Guid key)
        {
            try
            {
                return await _database.KeyDeleteAsync(key.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cache");
                return false;
            }
        }

        public string? GetStatus()
        {
            var endpoint = _database.Multiplexer.GetEndPoints().FirstOrDefault();
            if (endpoint == null)
                return "Undefined";

            var server = _database.Multiplexer.GetServer(endpoint);

            return server.IsConnected ? endpoint.ToString() : "Disconnected";
        }
    }
}
