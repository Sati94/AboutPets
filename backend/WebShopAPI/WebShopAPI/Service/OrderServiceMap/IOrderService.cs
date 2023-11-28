using WebShopAPI.Model.OrderModel;

namespace WebShopAPI.Service.OrderServiceMap
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrderAsync();
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<Order> GetOrderByUserId(string userId);
    }
}
