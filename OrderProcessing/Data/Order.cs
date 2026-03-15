namespace OrderProcessing.Data
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = [];
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string? CouponCode { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? PaymentTransactionId { get; set; }
        public string? IdempotencyKey { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum OrderStatus
    {
        Pending,
        StockReserved,
        Completed,
        PaymentFailed,
        Cancelled
    }
}
