using OrderProcessing.Data;

namespace OrderProcessing.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Product? GetById(Guid id);
        List<Product> GetAll();
        bool UpdateStock(Guid productId, int quantityChange);
    }
}
