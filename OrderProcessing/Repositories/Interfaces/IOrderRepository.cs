using OrderProcessing.Data;

namespace OrderProcessing.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Order? GetById(Guid id);
        Order? GetByIdempotencyKey(string key);
        List<Order> GetAll();
        void Add(Order order);
    }
}
