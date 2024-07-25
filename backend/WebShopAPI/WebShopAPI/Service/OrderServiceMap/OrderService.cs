using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using WebShopAPI.Data;
using WebShopAPI.Model;
using WebShopAPI.Model.OrderModel;
using WebShopAPI.Model.OrderModel.OrderStatus;
using WebShopAPI.Model.TodoItem;
using WebShopAPI.Service.NotificatonServiceMap;
using WebShopAPI.Model.OrderModel.DeliveryType;


namespace WebShopAPI.Service.OrderServiceMap
{
    public class OrderService : IOrderService
    {
        private readonly WebShopContext _context;
        private readonly INotificationService _notificationService;



        public OrderService(WebShopContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
         
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
        public async Task<IEnumerable<Order>> GetOrderByUserId(string userId)
        {
            var orderList = await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
            return orderList;
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
        public async Task<bool> UpdateOrderStatus(int orderId, [FromBody] int orderStatuses)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return false;
            }

            var userId = order.UserId;
            var previousStatus = order.OrderStatuses;
            var totalPrice = order.TotalPrice;
            decimal baseNumber = 100;
            
            order.OrderStatuses = (OrderStatuses)orderStatuses;


            try
            {
                
                if (previousStatus != order.OrderStatuses && order.OrderStatuses == OrderStatuses.Processing)
                {
                    var newOrderNotification = await _notificationService.NewOrderReceived(orderId);

                    _context.TodoItems.Add(newOrderNotification);
                }

                
                if (order.TotalPrice >= baseNumber)
                {
                    var bonusNotification = await _notificationService.CheckUserSpending(userId, orderId);
                    if (bonusNotification != null) 
                    {
                        _context.TodoItems.Add(bonusNotification);
                    }
                }

               
                await _context.SaveChangesAsync();

                return true; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something went wrong: {ex.Message}");
                return false; 
            }


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
        public async Task<bool> UpdateOrderDeliveryTypeAndAddress(int orderId, [FromBody] UpdateOrderDeliveryRequest request)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if(order == null)
            {
                return false;
            }

            order.DeliveryType = request.DeliveryType;
            order.Country = request.Country;
            order.City = request.City;
            order.StreetAddress = request.StreetAddress;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something is wrong is Updated Delivery and Address {ex.Message}");
                return false;
            }
        }
    }
}

