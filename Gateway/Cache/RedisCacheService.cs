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

        public RedisCacheService(IConnectionMultiplexer redis, IOptions<RedisSettings> options)
        {
            _database = redis.GetDatabase();
            _defaultExpiry = TimeSpan.FromMinutes(options.Value.DefaultCacheExpiration);
        }

        public async Task SetValueAsync<T>(Guid key, T value)
        {
            string json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key.ToString(), json, _defaultExpiry);
        }

        public async Task<T?> GetValueAsync<T>(Guid key)
        {
            string? json = await _database.StringGetAsync(key.ToString());
            return json is not null ? JsonSerializer.Deserialize<T>(json) : default;
        }
        public async Task<bool> DeleteAsync(Guid key)
        {
            return await _database.KeyDeleteAsync(key.ToString());
        }
        public string GetStatus()
        {
            return _database.Multiplexer.GetEndPoints().First().ToString() ?? "Undefinded";
        }
    }
}
