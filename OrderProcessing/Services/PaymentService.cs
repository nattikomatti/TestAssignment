using OrderProcessing.Integrations;
using OrderProcessing.Services.Interfaces;

namespace OrderProcessing.Services
{
    /// <summary>
    /// แยก Payment logic ออกจาก OrderService
    /// ทำให้ mock ได้ง่ายตอน test (inject IPaymentService แทน concrete PaymentGateway)
    /// </summary>
    public class PaymentService(PaymentGatewayClient gateway) : IPaymentService
    {
        public async Task<PaymentServiceResult> ProcessPaymentAsync(string customerName, decimal amount)
        {
            var result = await gateway.ChargeAsync(customerName, amount);
            return new PaymentServiceResult
            {
                Success = result.Success,
                TransactionId = result.TransactionId,
                ErrorMessage = result.ErrorMessage
            };
        }

        public async Task<bool> RefundPaymentAsync(string transactionId, decimal amount)
        {
            return await gateway.RefundAsync(transactionId, amount);
        }
    }
}
