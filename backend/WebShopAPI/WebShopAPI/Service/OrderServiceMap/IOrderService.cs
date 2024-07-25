using WebShopAPI.Model.OrderModel;
using WebShopAPI.Model.OrderModel.OrderStatus;
using WebShopAPI.Model;
using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Model.OrderModel.DeliveryType;

namespace WebShopAPI.Service.OrderServiceMap
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrderAsync();
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetOrderByUserId(string userId);
        Task<Order> DeleteOrderById (int orderId);
        Task<Order> GetOrderItemsByOrderIdAsync(int orderId);
        Task<Order> GetPendingOrders(string userId);
        Task<bool> UpdateOrderStatus(int orderId, [FromBody] int orderStatuses);
        Task<bool> UpdateOrderTotlaPriceWithBonus(int orderId, string userId );
        Task<bool> UpdateOrderDeliveryTypeAndAddress(int orderId,[FromBody] UpdateOrderDeliveryRequest request);
    }
}
