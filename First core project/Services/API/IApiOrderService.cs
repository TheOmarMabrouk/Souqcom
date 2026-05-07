using First_core_project.DTOs.API;

namespace First_core_project.Services.API
{
    public interface IApiOrderService
    {
        Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId);
        Task<int?> CreateOrderAsync(string userId);

        Task UpdateOrderStatusAsync(int orderId, int status);
    }
}
