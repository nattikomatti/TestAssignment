using OrderProcessing.Data;
using OrderProcessing.Repositories.Interfaces;

namespace OrderProcessing.Repositories
{
    public class OrderRepository(AppDbContext db) : IOrderRepository
    {
        public Order? GetById(Guid id) => db.Orders.FirstOrDefault(o => o.Id == id);

        public Order? GetByIdempotencyKey(string key) =>
            db.Orders.FirstOrDefault(o => o.IdempotencyKey == key && o.Status == OrderStatus.Completed);

        public List<Order> GetAll() => db.Orders;

        public void Add(Order order) => db.Orders.Add(order);
    }
}
