using WebShopAPI.Model;

namespace WebShopAPI.Service.OrderItemServiceMap
{
    public interface IOrderItemService
    {
        Task<OrderItem> AddOrderItemToUser(string userId, int productId, int quantity, int orderid);
        Task<OrderItem> DeleteOrderItem(int orderId, int orderItemId, string userId);
        Task<OrderItem> SetOrderItemQuantity(int orderId, int orderItemId , int newquantity);
    }
}
