using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NUnit.Framework;
using WebShopAPI.Service.NotificatonServiceMap;
using WebShopAPI.Model.TodoItem;
using WebShopAPI.Model.TodoItem.TodoItemStatus;
using WebShopAPI.Model;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace WebShopAPI.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet("/notifications/all"), Authorize(Roles = "Admin")]
        public async  Task<ActionResult<IEnumerable<TodoItem>>> GetAllTodo()
        {
            try
            {
                var result = await _notificationService.ListAllTodo();
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("notifications/deleteTodoItem/{todoItemId}")]
        public async Task<ActionResult<bool>> DeleteTodoItemController(int todoItemId)
        {
            try
            {
                var result = await _notificationService.DeleteTodoItem(todoItemId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPut("/updateTodoItemStatus/{todoItemId}")]
        public async Task<ActionResult<bool>> UpdateTodoItemStatusController(int todoItemId, TodoStatus status)
        {
            try
            {
                var result = await _notificationService.UpdatetodoItemStatus(todoItemId, status);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("/notifications/todo/addNotification")]
        public async Task<ActionResult<TodoItem>> AddNotificationController([FromBody]TodoModel model)
        {
            try
            {
                var todoItem = await _notificationService.AddNotification(model);
                return Ok(todoItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

}
