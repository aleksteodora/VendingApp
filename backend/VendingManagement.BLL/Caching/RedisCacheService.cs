using System.Text.Json;
using StackExchange.Redis;

namespace VendingManagement.BLL.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _database;
        private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(10);

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            var value = await _database.StringGetAsync(key);

            if (!value.HasValue)
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiry) where T : class
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiry);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task<T?> GetAsync<T>(string key, Func<Task<T?>> fallback, TimeSpan? expiry = null) where T : class
        {
            var cached = await GetAsync<T>(key);

            if (cached != null)
            {
                return cached;
            }

            var value = await fallback();

            if (value != null)
            {
                await SetAsync(key, value, expiry ?? DefaultExpiry);
            }

            return value;
        }
    }
}