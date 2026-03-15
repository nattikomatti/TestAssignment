using OrderProcessing.Data;
using OrderProcessing.Infrastructure;
using OrderProcessing.Repositories.Interfaces;
using OrderProcessing.Services.Interfaces;

namespace OrderProcessing.Services
{
    /// <summary>
    /// แยก Inventory logic ออกจาก OrderService เพื่อให้ test ง่าย
    /// ใช้ Redis cache สำหรับ product lookup → รองรับ 3,000 order/นาที
    /// </summary>
    public class InventoryService(IProductRepository productRepo, RedisCacheService cache) : IInventoryService
    {
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(5);

        public async Task<Product?> GetProductAsync(Guid productId)
        {
            var cacheKey = $"product:{productId}";
            var cached = await cache.GetAsync<Product>(cacheKey);
            if (cached is not null) return cached;

            var product = productRepo.GetById(productId);
            if (product is not null)
                await cache.SetAsync(cacheKey, product, CacheExpiry);

            return product;
        }

        public Task<List<Product>> GetAllProductsAsync()
        {
            return Task.FromResult(productRepo.GetAll());
        }

        public bool CheckStock(Guid productId, int quantity)
        {
            var product = productRepo.GetById(productId);
            return product is not null && product.StockQuantity >= quantity;
        }

        public bool ReserveStock(Guid productId, int quantity)
        {
            return productRepo.UpdateStock(productId, -quantity);
        }

        public void ReleaseStock(Guid productId, int quantity)
        {
            productRepo.UpdateStock(productId, quantity);
        }
    }
}
