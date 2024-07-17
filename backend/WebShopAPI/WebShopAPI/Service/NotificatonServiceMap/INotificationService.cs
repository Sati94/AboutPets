using WebShopAPI.Model.TodoItem;
using WebShopAPI.Model.TodoItem.TodoItemStatus;

namespace WebShopAPI.Service.NotificatonServiceMap
{
    public interface INotificationService
    {
        Task<TodoItem> CheckUserSpending(string userId, int orderId);
        Task<TodoItem> CheckProductStock(int productId);
        Task<TodoItem> NewOrderReceived(int orderId);
        Task<bool> DeleteTodoItem(int todoItemId);
        Task<bool> UpdatetodoItemStatus(int todoItemId, TodoStatus status);
        Task<TodoItem> AddNotification(TodoModel todoModel);
    }
        



        
  
}
