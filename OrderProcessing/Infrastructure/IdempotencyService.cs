namespace OrderProcessing.Infrastructure
{
    /// <summary>
    /// แก้ปัญหา: Order ถูก charge ซ้ำ
    /// ใช้ Redis เก็บ idempotency key เพื่อป้องกันการสร้าง order ซ้ำ
    /// ถ้า request มี key เดิม จะคืนผลลัพธ์ที่ cache ไว้แทนการประมวลผลใหม่
    /// </summary>
    public class IdempotencyService(RedisCacheService cache)
    {
        private static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(24);

        public async Task<bool> IsProcessedAsync(string key)
        {
            var result = await cache.GetStringAsync($"idempotency:{key}");
            return result is not null;
        }

        public async Task<T?> GetResultAsync<T>(string key)
        {
            return await cache.GetAsync<T>($"idempotency:{key}");
        }

        public async Task MarkProcessedAsync<T>(string key, T result, TimeSpan? expiry = null)
        {
            await cache.SetAsync($"idempotency:{key}", result, expiry ?? DefaultExpiry);
        }
    }
}
