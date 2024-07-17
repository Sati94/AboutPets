using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using WebShopAPI.Data;
using WebShopAPI.Model;
using WebShopAPI.Model.OrderModel.OrderStatus;
using WebShopAPI.Model.TodoItem;
using WebShopAPI.Model.TodoItem.TodoItemStatus;




namespace WebShopAPI.Service.NotificatonServiceMap
{
    public class NotificationService : INotificationService
    {
        private readonly WebShopContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        public NotificationService(WebShopContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<TodoItem>> ListAllTodo()
        {
            var todos = await _context.TodoItems.ToListAsync();

            return todos;
        }
        public async Task<TodoItem> CheckUserSpending(string userId, int orderId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                return null;
            }
            decimal baseNumber = 100;
            var totalPrice = order.TotalPrice;

            if (totalPrice >= baseNumber)
            {
                var todoItem = CreateTodoItem("System", "Bonus Alert", $"User {user.UserName}, userId : {user.Id} spent over $100. Give them a bonus!");
               
             
                return todoItem;
            }
           return null;
        }
        public async Task<TodoItem> CheckProductStock(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product != null && product.Stock <= 10)
            {
                var todoItem = CreateTodoItem("System", "Stock Alert", $"Product {product.ProductName} stock is low. Now it is {product.Stock}.You need to talk to the warehouse!");

                _context.TodoItems.Add(todoItem);
                await _context.SaveChangesAsync();
                
                return todoItem;
            }
            return null;
        }
        public async Task<TodoItem> NewOrderReceived(int orderId)
        {
         

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if(order == null)
            {
                throw new ArgumentException("Order not found !");
            }

            var user = await _userManager.FindByIdAsync(order.UserId);

            if(user == null) 
            {
                throw new ArgumentException("User not foundd !");
            }
            var todoItem = CreateTodoItem("System", "New Order", $"New order received from {user.Id}");

           

            return todoItem;
        }

        public async Task<bool> DeleteTodoItem(int todoItemId)
        {
            var todoItem = await _context.TodoItems.FindAsync(todoItemId);

            if(todoItem == null)
            {
                throw new ArgumentException("Todo item not found");
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdatetodoItemStatus(int todoItemId, TodoStatus status)
        {
            var todoItem = await _context.TodoItems.FindAsync(todoItemId);
            if(todoItem == null)
            {
                throw new ArgumentException("Todo item not found!");
            };

            todoItem.Status = status;
            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<TodoItem> AddNotification(TodoModel todoModel)
        {
            var todoItem = CreateTodoItem(todoModel.Sender, todoModel.Title, todoModel.Description);

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            return todoItem;
        }

        private TodoItem CreateTodoItem(string sender,string title, string description)
        {
            return  new TodoItem
            {
                Sender = sender,
                Title = title,
                Description = description,
                CreatedDate = DateTime.UtcNow,
                Status = TodoStatus.New,
            }; 
            
        }
       
    }
}
