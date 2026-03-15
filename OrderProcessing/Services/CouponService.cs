using OrderProcessing.Data;
using OrderProcessing.Services.Interfaces;

namespace OrderProcessing.Services
{
    /// <summary>
    /// แยก Coupon logic ออกจาก OrderService
    /// แก้ปัญหา: coupon hard-coded (DIS10, DIS20) → ใช้ data-driven จาก DB
    /// รองรับ validation ครบ: หมดอายุ, ใช้ครบจำนวน, ยอดขั้นต่ำ
    /// </summary>
    public class CouponService(AppDbContext db) : ICouponService
    {
        public CouponResult ValidateAndCalculate(string couponCode, decimal orderAmount)
        {
            var coupon = db.Coupons.FirstOrDefault(c =>
                c.Code.Equals(couponCode, StringComparison.OrdinalIgnoreCase));

            if (coupon is null)
                return new CouponResult { IsValid = false, ErrorMessage = "ไม่พบคูปอง" };

            if (!coupon.IsActive)
                return new CouponResult { IsValid = false, ErrorMessage = "คูปองไม่ได้เปิดใช้งาน" };

            if (DateTime.UtcNow > coupon.ExpiryDate)
                return new CouponResult { IsValid = false, ErrorMessage = "คูปองหมดอายุแล้ว" };

            if (coupon.UsedCount >= coupon.MaxUsage)
                return new CouponResult { IsValid = false, ErrorMessage = "คูปองถูกใช้ครบจำนวนแล้ว" };

            if (orderAmount < coupon.MinOrderAmount)
                return new CouponResult { IsValid = false, ErrorMessage = $"ยอดสั่งซื้อขั้นต่ำ {coupon.MinOrderAmount:N0} บาท" };

            var discount = orderAmount * (coupon.DiscountPercent / 100m);
            if (discount > coupon.MaxDiscountAmount)
                discount = coupon.MaxDiscountAmount;

            coupon.UsedCount++;

            return new CouponResult { IsValid = true, DiscountAmount = discount };
        }
    }
}
