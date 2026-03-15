using StackExchange.Redis;
using System.Text.Json;

namespace OrderProcessing.Infrastructure
{
    public class RedisCacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(value.ToString()!);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            if (expiry.HasValue)
                await _db.StringSetAsync(key, json, expiry.Value);
            else
                await _db.StringSetAsync(key, json);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task<bool> SetNxAsync(string key, string value, TimeSpan expiry)
        {
            return await _db.StringSetAsync(key, value, expiry, When.NotExists);
        }

        public async Task<string?> GetStringAsync(string key)
        {
            var value = await _db.StringGetAsync(key);
            return value.IsNullOrEmpty ? null : value.ToString();
        }

        public async Task<bool> DeleteIfValueMatchesAsync(string key, string expectedValue)
        {
            var script = @"
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end";
            var result = await _db.ScriptEvaluateAsync(script,
                [(RedisKey)key], [(RedisValue)expectedValue]);
            return (int)result == 1;
        }
    }
}
