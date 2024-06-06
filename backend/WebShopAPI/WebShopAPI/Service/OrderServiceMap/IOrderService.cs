using WebShopAPI.Model.OrderModel;
using WebShopAPI.Model.OrderModel.OrderStatus;
using WebShopAPI.Model;

namespace WebShopAPI.Service.OrderServiceMap
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrderAsync();
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<Order> GetOrderByUserId(string userId);
        Task<Order> DeleteOrderById (int orderId);
        Task<Order> GetOrderItemsByOrderIdAsync(int orderId);
        Task<Order> GetPendingOrders(string userId);
        Task<bool> UpdateOrderStatus(int orderId, OrderStatuses newStatus);
        Task<bool> UpdateOrderTotlaPriceWithBonus(int orderId, string userId );
    }
}
