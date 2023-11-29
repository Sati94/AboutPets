using WebShopAPI.Model.OrderModel;
using WebShopAPI.Model.OrderModel.OrderStatus;

namespace WebShopAPI.Service.OrderServiceMap
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrderAsync();
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<Order> GetOrderByUserId(string userId);
        Task<Order> DeleteOrderById (int orderId);
        Task<bool> UpdateOrderStatus(int orderId, OrderStatuses newStatus);
    }
}
