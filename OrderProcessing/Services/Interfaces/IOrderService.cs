using OrderProcessing.Models.Requests;
using OrderProcessing.Models.Responses;

namespace OrderProcessing.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
        OrderResponse GetById(Guid id);
        Task<List<OrderDetailResponse>> GetAll();
        Task<OrderResponse> CancelOrderAsync(Guid id);
    }
}
