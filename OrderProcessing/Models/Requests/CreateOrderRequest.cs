namespace OrderProcessing.Models.Requests
{
    public class CreateOrderRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string? CouponCode { get; set; }
        public string? IdempotencyKey { get; set; }
        public List<OrderItemRequest> Items { get; set; } = [];
    }

    public class OrderItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
