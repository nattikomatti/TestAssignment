namespace OrderProcessing.Models.Responses
{
    public class OrderResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public OrderDetailResponse? Data { get; set; }
    }

    public class OrderDetailResponse
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string? CouponCode { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentTransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDetailResponse> Items { get; set; } = [];
    }

    public class OrderItemDetailResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}
