using Microsoft.EntityFrameworkCore;
using WebShopAPI.Data;
using WebShopAPI.Model;
using WebShopAPI.Model.OrderModel;
using WebShopAPI.Model.OrderModel.OrderStatus; 

namespace WebShopAPI.Service.OrderItemServiceMap
{
    public class OrderItemService : IOrderItemService
    {
        private readonly WebShopContext _context;
        public OrderItemService(WebShopContext context)
        {
            _context = context;
        }
        public async Task<OrderItem> AddOrderItemToUser(string userId, int productId, int quantity)
        {
            var user = await _context.Useres.FirstOrDefaultAsync(u => u.Id == userId);
            if(user == null)
            {
                throw new ArgumentException("User not found");
            }

            var product = await _context.Products.FirstOrDefaultAsync(prod => prod.ProductId == productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }
            var order = user.Orders.FirstOrDefault();
            if (order == null)
            {
                order = new Order
                {
                    OrderDate = DateTime.Now,
                    TotalPrice = 0,
                    OrderStatuses = OrderStatuses.Pending
                };
                user.Orders.Add(order);
            }

            var orderItem = new OrderItem
            {
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price * quantity
            };
            if(product.Stock < orderItem.Quantity)
            {
                throw new ArgumentException("Requested quantity exceeds available stock");
            }
            
            product.Stock = product.Stock - quantity;
            order.OrderItems.Add(orderItem);
            user.OrderItems.Add(orderItem);
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
            return orderItem;
        }
    }
}
