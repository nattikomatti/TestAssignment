namespace OrderProcessing.Data
{
    public class AppDbContext
    {
        public List<Product> Products { get; } = [];
        public List<Order> Orders { get; } = [];
        public List<Coupon> Coupons { get; } = [];

        public AppDbContext()
        {
            SeedData();
        }

        private void SeedData()
        {
            var productId1 = Guid.Parse("f0000001-0001-0000-0000-000000000001");
            var productId2 = Guid.Parse("f0000001-0002-0000-0000-000000000002");
            var productId3 = Guid.Parse("f0000001-0003-0000-0000-000000000003");
            var productId4 = Guid.Parse("f0000001-0004-0000-0000-000000000004");

            Products.AddRange([
                new Product { Id = productId1, Name = "MacBook Pro 14\"", Description = "Apple M3 Pro, 18GB RAM, 512GB SSD", Price = 69900m, StockQuantity = 25 },
                new Product { Id = productId2, Name = "iPhone 16 Pro", Description = "256GB, Natural Titanium", Price = 42900m, StockQuantity = 50 },
                new Product { Id = productId3, Name = "AirPods Pro 2", Description = "USB-C, Active Noise Cancellation", Price = 8990m, StockQuantity = 100 },
                new Product { Id = productId4, Name = "iPad Air M2", Description = "11 inch, 256GB, Wi-Fi", Price = 24900m, StockQuantity = 3 },
            ]);

            Coupons.AddRange([
                new Coupon { Code = "DIS10", Description = "ส่วนลด 10%", DiscountPercent = 10, MaxDiscountAmount = 5000, MinOrderAmount = 1000, MaxUsage = 100, ExpiryDate = DateTime.UtcNow.AddDays(30) },
                new Coupon { Code = "DIS20", Description = "ส่วนลด 20%", DiscountPercent = 20, MaxDiscountAmount = 10000, MinOrderAmount = 5000, MaxUsage = 50, ExpiryDate = DateTime.UtcNow.AddDays(15) },
                new Coupon { Code = "EXPIRED01", Description = "คูปองหมดอายุ", DiscountPercent = 30, MaxDiscountAmount = 15000, MinOrderAmount = 1000, MaxUsage = 10, ExpiryDate = DateTime.UtcNow.AddDays(-5), IsActive = false },
            ]);

            var orderId1 = Guid.Parse("d0000001-0001-0000-0000-000000000001");
            Orders.Add(new Order
            {
                Id = orderId1,
                CustomerName = "สมชาย ใจดี",
                TotalAmount = 69900m,
                DiscountAmount = 0,
                FinalAmount = 69900m,
                Status = OrderStatus.Completed,
                PaymentTransactionId = "TXN-SEED-001",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                Items =
                [
                    new OrderItem { OrderId = orderId1, ProductId = productId1, ProductName = "MacBook Pro 14\"", UnitPrice = 69900m, Quantity = 1 }
                ]
            });
        }
    }
}
