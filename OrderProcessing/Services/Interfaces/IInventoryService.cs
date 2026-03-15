using OrderProcessing.Data;

namespace OrderProcessing.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<Product?> GetProductAsync(Guid productId);
        Task<List<Product>> GetAllProductsAsync();
        bool CheckStock(Guid productId, int quantity);
        bool ReserveStock(Guid productId, int quantity);
        void ReleaseStock(Guid productId, int quantity);
    }
}
