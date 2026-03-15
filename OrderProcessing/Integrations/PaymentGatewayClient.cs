namespace OrderProcessing.Integrations
{
    public class PaymentGatewayClient
    {
        public Task<PaymentResult> ChargeAsync(string customerName, decimal amount)
        {
            Console.WriteLine($"[Payment Gateway] Charging {amount:N2} THB for customer: {customerName}");
            var transactionId = $"TXN-{Guid.NewGuid():N}"[..20].ToUpper();
            return Task.FromResult(new PaymentResult { Success = true, TransactionId = transactionId });
        }

        public Task<bool> RefundAsync(string transactionId, decimal amount)
        {
            Console.WriteLine($"[Payment Gateway] Refunding {amount:N2} THB, TxnId: {transactionId}");
            return Task.FromResult(true);
        }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }
}
