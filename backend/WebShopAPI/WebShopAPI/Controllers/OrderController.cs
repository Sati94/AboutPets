using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using WebShopAPI.Service.OrderServiceMap;
using WebShopAPI.Model.OrderModel;
using WebShopAPI.Model.OrderModel.OrderStatus;

namespace WebShopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet("/orderlist/all"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrder()
        {
            try
            {
                var result = await _orderService.GetAllOrderAsync();
                return Ok(result);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("/ordelist/order/{orderId}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<Order>> GetOrderById(int orderId) 
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                return Ok(order);
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/order/{userId}"), Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<Order>> GetOrderByUserId(string userId)
        {
            try
            {
                var order = await _orderService.GetOrderByUserId(userId);
                return Ok(order);
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("/order/delete/{orderId}"), Authorize(Roles ="Admin,User")]
        public async Task<ActionResult<Order>> DeleteOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _orderService.DeleteOrderById(orderId);
                return Ok(order);
            }
            catch(ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("/order/update/{orderId}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> UpdateStatusByOrderId(int orderId, OrderStatuses orderstatus)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatus(orderId, orderstatus);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);  
            }
        }
        
    }
}
