namespace OrderProcessing.Data
{
    public class Coupon
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public decimal MaxDiscountAmount { get; set; }
        public decimal MinOrderAmount { get; set; }
        public bool IsActive { get; set; } = true;
        public int UsedCount { get; set; }
        public int MaxUsage { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
