namespace OrderProcessing.Services.Interfaces
{
    public interface ICouponService
    {
        CouponResult ValidateAndCalculate(string couponCode, decimal orderAmount);
    }

    public class CouponResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}
