using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;
        public OrderItemService(WebShopContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        public async Task<OrderItem> AddOrderItemToUser(string userId, int productId, int quantity, int orderId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var product = await _context.Products.FirstOrDefaultAsync(prod => prod.ProductId == productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }


            var order = await _context.Orders.Include(o => o.OrderItems)
                                       .FirstOrDefaultAsync(or => or.UserId == userId && or.OrderStatuses == OrderStatuses.Pending);
            if (order != null)
            {
                orderId = order.OrderId;
            }



            if (order == null)
            {
                order = new Order
                {
                    OrderDate = DateTime.Now,
                    TotalPrice = 0,
                    OrderStatuses = OrderStatuses.Pending,
                    UserId = userId,
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }


            var orderItem = new OrderItem
            {
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price * quantity
            };
            if (product.Stock < orderItem.Quantity)
            {
                throw new ArgumentException("Requested quantity exceeds available stock!");
            }

            product.Stock = product.Stock - quantity;

            order.OrderItems.Add(orderItem);
            _context.OrderItems.Add(orderItem);



            order.TotalPrice = order.OrderItems.Sum(oi => oi.Price);
            await _context.SaveChangesAsync();
            return orderItem;
        }
        public async Task<OrderItem> DeleteOrderItem(int orderId, int orderItemId, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var order = await _context.Orders
                          .Include(o => o.OrderItems)
                          .ThenInclude(oi => oi.Product) 
                          .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
                if (order != null)
                {
                    var orderItem = order.OrderItems.FirstOrDefault(oi => oi.OrderItemId == orderItemId);

                    if (orderItem != null)
                    {
                        var product = orderItem.Product;
                        _context.OrderItems.Remove(orderItem);
                        if (product != null)
                        {
               
                            product.Stock += orderItem.Quantity;
                            _context.Products.Update(product);

                        }
                        order.OrderItems.Remove(orderItem);


                        order.TotalPrice -= orderItem.Price;

                        if (order.OrderItems.Count == 0)
                        {
                            _context.Orders.Remove(order);
                        }

                        // OrderItem törlése az adatbázisból
                        _context.OrderItems.Remove(orderItem);

                        // Adatok mentése
                        await _context.SaveChangesAsync();

                        return orderItem;
                    }
                }
            }

            return null;

        }
        public async Task<OrderItem> SetOrderItemQuantity(int orderId, int orderItemId, int newquantity)
        {
            var order = await _context.Orders.FindAsync(orderId);
            var userId = order.UserId;
            if (order != null && userId != null)
            {
                var orderItem = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);


                if (orderItem != null && order.OrderItems.Contains(orderItem))
                {
                    var oldQuantity = orderItem.Quantity;
                    var difference = newquantity - oldQuantity;
                    orderItem.Quantity = newquantity;
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.OrderItems.Contains(orderItem));
                    if (product != null)
                    {
                        orderItem.Price = product.Price * newquantity;
                        if (difference > 0)
                        {
                            product.Stock -= difference;
                        }
                        else if (difference < 0)
                        {
                            product.Stock += Math.Abs(difference);
                        }


                        var orders = await _context.Orders.FirstOrDefaultAsync(o => o.OrderItems.Any(oi => oi.OrderItemId == orderItemId));
                        if (orders != null)
                        {
                            orders.TotalPrice = orders.OrderItems.Sum(oi => oi.Price);
                        }
                        if (newquantity == 0)
                        {
                            _context.OrderItems.Remove(orderItem);
                            _context.Orders.Remove(order);
                            await _context.SaveChangesAsync();
                        }

                        await _context.SaveChangesAsync();
                        return orderItem;
                    }
                }
            }
            return null;
        }
    
    }
}
