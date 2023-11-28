using Microsoft.EntityFrameworkCore;
using WebShopAPI.Data;
using WebShopAPI.Model.OrderModel;

namespace WebShopAPI.Service.OrderServiceMap
{
    public class OrderService : IOrderService
    {
        private readonly WebShopContext _context;
        public OrderService(WebShopContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Order>> GetAllOrderAsync()
        {
            var orderList = await _context.Orders.ToListAsync();
            return orderList;
        }
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o=> o.OrderId == orderId);
            return order;
        }
    }
}
