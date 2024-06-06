using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebShopAPI.Data;
using WebShopAPI.Model;
using WebShopAPI.Model.OrderModel;
using WebShopAPI.Model.OrderModel.OrderStatus;


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
        public async Task<Order> GetPendingOrders(string userId)
        {

            var pendingOrder = await _context.Orders
           .FirstOrDefaultAsync(o => o.OrderStatuses == OrderStatuses.Pending && o.UserId == userId);
            if(pendingOrder == null)
            {
                return null;
            }
            return pendingOrder;
           
           
    
        }
        public async Task<Order>GetOrderItemsByOrderIdAsync(int orderId)
        {
           var order =  await _context.Orders
           .Include(o => o.OrderItems)
           .ThenInclude(oi => oi.Product)
           .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if(order == null)
            {
                return null;
            }
            return order;
        }
        public async Task<bool> UpdateOrderStatus(int orderId, OrderStatuses newStatus)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                order.OrderStatuses = newStatus;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateOrderTotlaPriceWithBonus(int orderId, string userId)
        {
           
                var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
            if (order == null) 
            {
                return false;
            }
            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
            if (userProfile != null)
            {
                var cupon = userProfile.Bonus;
                if(cupon > 0)
                {
                order.TotalPrice -= order.TotalPrice * cupon;
                userProfile.Bonus -= cupon;
                _context.UserProfiles.Update(userProfile);
                }

                await _context.SaveChangesAsync();
                return true;


            }
            return false;
          
        }
    }
}

