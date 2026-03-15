using OrderProcessing.Data;
using OrderProcessing.Repositories.Interfaces;

namespace OrderProcessing.Repositories
{
    public class ProductRepository(AppDbContext db) : IProductRepository
    {
        public Product? GetById(Guid id) => db.Products.FirstOrDefault(p => p.Id == id);

        public List<Product> GetAll() => db.Products;

        public bool UpdateStock(Guid productId, int quantityChange)
        {
            var product = GetById(productId);
            if (product is null || product.StockQuantity + quantityChange < 0)
                return false;

            product.StockQuantity += quantityChange;
            return true;
        }
    }
}
