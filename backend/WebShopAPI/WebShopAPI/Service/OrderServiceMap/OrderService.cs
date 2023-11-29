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
        public async Task<Order> GetOrderByUserId(string userId)
        {
            var order = _context.Orders.FirstOrDefault(o=> o.UserId == userId);
            return order;
        }
        public async Task<Order> DeleteOrderById(int orderId)
        {
            var order = _context.Orders.Include(o => o.OrderItems).ThenInclude(oi=> oi.Product).FirstOrDefault(o => o.OrderId == orderId);

            if (order != null)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    if (orderItem != null)
                    {
                        var product = orderItem.Product;
                        _context.OrderItems.Remove(orderItem);
                        if(product != null)
                        {
                           Console.WriteLine($"Product information: ProductId: {product.ProductId}, ProductName: {product.ProductName}, Stock: {product.Stock}");
                            product.Stock += orderItem.Quantity;
                            _context.Products.Update(product);
                            
                        }
                    }  
                }
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }

                return order;   
        }
    }
}
