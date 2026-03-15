using OrderProcessing.Data;
using OrderProcessing.Infrastructure;
using OrderProcessing.Models.Requests;
using OrderProcessing.Models.Responses;
using OrderProcessing.Repositories.Interfaces;
using OrderProcessing.Services.Interfaces;

namespace OrderProcessing.Services
{
    /// <summary>
    /// Refactored OrderService — แก้ปัญหาทั้ง 4:
    /// 1. Stock ติดลบ         → DistributedLock (Redis SETNX) ป้องกัน race condition
    /// 2. Charge ซ้ำ          → IdempotencyService (Redis) ป้องกัน duplicate order
    /// 3. Order บันทึก/payment ล้มเหลว → Saga compensation: rollback stock ถ้า payment fail
    /// 4. Test ยาก            → แยก service + interface + DI ทำให้ mock ได้ง่าย
    /// </summary>
    public class OrderService(
        IOrderRepository orderRepo,
        IInventoryService inventoryService,
        ICouponService couponService,
        IPaymentService paymentService,
        DistributedLockService lockService,
        IdempotencyService idempotencyService) : IOrderService
    {
        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            // ═══════════════════════════════════════════════════════════
            // 1. Idempotency Check (แก้: charge ซ้ำ)
            //    ถ้า request เดิมเคยสำเร็จแล้ว → คืนผลลัพธ์เดิมจาก Redis
            // ═══════════════════════════════════════════════════════════
            if (!string.IsNullOrEmpty(request.IdempotencyKey))
            {
                var existing = await idempotencyService.GetResultAsync<OrderResponse>(request.IdempotencyKey);
                if (existing is not null)
                {
                    existing.Message = "คำสั่งซื้อนี้ถูกประมวลผลไปแล้ว (idempotent)";
                    return existing;
                }
            }

            // ═══════════════════════════════════════════════════════════
            // 2. Validate Request
            // ═══════════════════════════════════════════════════════════
            if (string.IsNullOrWhiteSpace(request.CustomerName))
                return Fail("กรุณาระบุชื่อลูกค้า");

            if (request.Items is null || request.Items.Count == 0)
                return Fail("กรุณาเพิ่มสินค้าอย่างน้อย 1 รายการ");

            // ═══════════════════════════════════════════════════════════
            // 3. Distributed Lock (แก้: stock ติดลบ)
            //    ล็อกตาม product IDs → ป้องกัน concurrent deduct stock
            // ═══════════════════════════════════════════════════════════
            var lockResource = string.Join(",",
                request.Items.Select(i => i.ProductId).OrderBy(id => id));

            await using var distributedLock = await lockService.AcquireLockAsync(
                $"order:{lockResource}", TimeSpan.FromSeconds(30));

            if (distributedLock is null)
                return Fail("ระบบไม่ว่าง กรุณาลองใหม่อีกครั้ง");

            // ═══════════════════════════════════════════════════════════
            // 4. Check stock & build order items (ภายใน lock)
            // ═══════════════════════════════════════════════════════════
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in request.Items)
            {
                var product = await inventoryService.GetProductAsync(item.ProductId);
                if (product is null)
                    return Fail($"ไม่พบสินค้า ID: {item.ProductId}");

                if (!inventoryService.CheckStock(item.ProductId, item.Quantity))
                    return Fail($"สินค้า '{product.Name}' มีไม่เพียงพอ (เหลือ {product.StockQuantity} ชิ้น)");

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = item.Quantity
                });

                totalAmount += product.Price * item.Quantity;
            }

            // ═══════════════════════════════════════════════════════════
            // 5. Reserve stock (ภายใน lock — atomic)
            // ═══════════════════════════════════════════════════════════
            var reservedItems = new List<OrderItemRequest>();
            foreach (var item in request.Items)
            {
                if (!inventoryService.ReserveStock(item.ProductId, item.Quantity))
                {
                    // Rollback ที่จองไปแล้ว
                    foreach (var reserved in reservedItems)
                        inventoryService.ReleaseStock(reserved.ProductId, reserved.Quantity);
                    return Fail("ไม่สามารถจองสินค้าได้");
                }
                reservedItems.Add(item);
            }

            // ═══════════════════════════════════════════════════════════
            // 6. Apply Coupon (data-driven แทน hard-coded)
            // ═══════════════════════════════════════════════════════════
            decimal discountAmount = 0;
            if (!string.IsNullOrEmpty(request.CouponCode))
            {
                var couponResult = couponService.ValidateAndCalculate(request.CouponCode, totalAmount);
                if (!couponResult.IsValid)
                {
                    // Compensation: คืน stock
                    foreach (var item in request.Items)
                        inventoryService.ReleaseStock(item.ProductId, item.Quantity);
                    return Fail($"คูปองไม่ถูกต้อง: {couponResult.ErrorMessage}");
                }
                discountAmount = couponResult.DiscountAmount;
            }

            var finalAmount = totalAmount - discountAmount;

            // ═══════════════════════════════════════════════════════════
            // 7. Create Order entity (ยังไม่บันทึกจนกว่า payment สำเร็จ)
            // ═══════════════════════════════════════════════════════════
            var order = new Order
            {
                CustomerName = request.CustomerName,
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount,
                CouponCode = request.CouponCode,
                IdempotencyKey = request.IdempotencyKey,
                Status = OrderStatus.StockReserved
            };

            foreach (var item in orderItems)
                item.OrderId = order.Id;
            order.Items = orderItems;

            // ═══════════════════════════════════════════════════════════
            // 8. Process Payment (แก้: order บันทึกแต่ payment ล้มเหลว)
            //    ถ้า payment fail → compensation: คืน stock ทั้งหมด
            // ═══════════════════════════════════════════════════════════
            var paymentResult = await paymentService.ProcessPaymentAsync(
                request.CustomerName, finalAmount);

            if (!paymentResult.Success)
            {
                // Saga compensation: คืน stock กลับ
                foreach (var item in request.Items)
                    inventoryService.ReleaseStock(item.ProductId, item.Quantity);

                order.Status = OrderStatus.PaymentFailed;
                orderRepo.Add(order);

                return Fail($"การชำระเงินล้มเหลว: {paymentResult.ErrorMessage ?? "กรุณาลองใหม่"}");
            }

            // ═══════════════════════════════════════════════════════════
            // 9. Payment success → บันทึก order สถานะ Completed
            // ═══════════════════════════════════════════════════════════
            order.PaymentTransactionId = paymentResult.TransactionId;
            order.Status = OrderStatus.Completed;
            orderRepo.Add(order);

            var successResponse = new OrderResponse
            {
                Success = true,
                Message = "สั่งซื้อสำเร็จ",
                Data = MapToDetail(order)
            };

            // ═══════════════════════════════════════════════════════════
            // 10. Mark Idempotency (ป้องกัน charge ซ้ำ)
            // ═══════════════════════════════════════════════════════════
            if (!string.IsNullOrEmpty(request.IdempotencyKey))
                await idempotencyService.MarkProcessedAsync(request.IdempotencyKey, successResponse);

            return successResponse;
        }

        public OrderResponse GetById(Guid id)
        {
            var order = orderRepo.GetById(id);
            if (order is null)
                return new OrderResponse { Success = false, Message = "ไม่พบคำสั่งซื้อ" };

            return new OrderResponse { Success = true, Data = MapToDetail(order) };
        }

        public List<OrderDetailResponse> GetAll()
        {
            return orderRepo.GetAll().Select(MapToDetail).ToList();
        }

        public async Task<OrderResponse> CancelOrderAsync(Guid id)
        {
            var order = orderRepo.GetById(id);
            if (order is null)
                return new OrderResponse { Success = false, Message = "ไม่พบคำสั่งซื้อ" };

            if (order.Status == OrderStatus.Cancelled)
                return new OrderResponse { Success = false, Message = "คำสั่งซื้อถูกยกเลิกไปแล้ว" };

            // Refund payment ถ้า order เคย charge แล้ว
            if (order.Status == OrderStatus.Completed && !string.IsNullOrEmpty(order.PaymentTransactionId))
            {
                var refunded = await paymentService.RefundPaymentAsync(
                    order.PaymentTransactionId, order.FinalAmount);
                if (!refunded)
                    return new OrderResponse { Success = false, Message = "ไม่สามารถคืนเงินได้ กรุณาติดต่อ admin" };
            }

            // คืน stock
            foreach (var item in order.Items)
                inventoryService.ReleaseStock(item.ProductId, item.Quantity);

            order.Status = OrderStatus.Cancelled;

            return new OrderResponse
            {
                Success = true,
                Message = "ยกเลิกคำสั่งซื้อสำเร็จ" +
                    (order.PaymentTransactionId is not null ? " (คืนเงินเรียบร้อย)" : ""),
                Data = MapToDetail(order)
            };
        }

        private static OrderResponse Fail(string message) =>
            new() { Success = false, Message = message };

        private static OrderDetailResponse MapToDetail(Order order) => new()
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            TotalAmount = order.TotalAmount,
            DiscountAmount = order.DiscountAmount,
            FinalAmount = order.FinalAmount,
            CouponCode = order.CouponCode,
            Status = order.Status.ToString(),
            PaymentTransactionId = order.PaymentTransactionId,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemDetailResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                SubTotal = i.SubTotal
            }).ToList()
        };
    }
}
