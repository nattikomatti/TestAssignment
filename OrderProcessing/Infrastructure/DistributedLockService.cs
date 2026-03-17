namespace OrderProcessing.Infrastructure
{
 
    /// แก้ปัญหา: Stock ติดลบ
    /// ใช้ Redis SETNX เป็น distributed lock ป้องกัน race condition
    /// เมื่อหลาย request สั่งซื้อสินค้าเดียวกันพร้อมกัน
   
    public class DistributedLockService(RedisCacheService cache)
    {
        public async Task<DistributedLock?> AcquireLockAsync(
            string resource,
            TimeSpan expiry,
            TimeSpan? waitTime = null,
            TimeSpan? retryInterval = null)
        {
            var lockKey = $"lock:{resource}";
            var lockValue = Guid.NewGuid().ToString();
            var wait = waitTime ?? TimeSpan.FromSeconds(10);
            var retry = retryInterval ?? TimeSpan.FromMilliseconds(100);
            var deadline = DateTime.UtcNow.Add(wait);

            while (DateTime.UtcNow < deadline)
            {
                if (await cache.SetNxAsync(lockKey, lockValue, expiry))
                    return new DistributedLock(lockKey, lockValue, cache);

                await Task.Delay(retry);
            }

            return null;
        }
    }

    public class DistributedLock(string key, string value, RedisCacheService cache) : IAsyncDisposable
    {
        public async ValueTask DisposeAsync()
        {
            await cache.DeleteIfValueMatchesAsync(key, value);
        }
    }
}
