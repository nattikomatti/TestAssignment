namespace OrderProcessing.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentServiceResult> ProcessPaymentAsync(string customerName, decimal amount);
        Task<bool> RefundPaymentAsync(string transactionId, decimal amount);
    }

    public class PaymentServiceResult
    {
        public bool Success { get; set; }
        public string? TransactionId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
