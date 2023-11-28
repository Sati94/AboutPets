using WebShopAPI.Model;

namespace WebShopAPI.Service.OrderItemServiceMap
{
    public interface IOrderItemService
    {
        Task<OrderItem> AddOrderItemToUser(string userId, int productId, int quantity, int orderid);
        Task<OrderItem> DeleteOrderItem(string userId, int orderItemId);
    }
}
