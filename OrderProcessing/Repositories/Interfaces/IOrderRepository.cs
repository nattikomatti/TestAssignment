using OrderProcessing.Data;

namespace OrderProcessing.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Order? GetById(Guid id);
        List<Order> GetAll();
        void Add(Order order);
    }
}
