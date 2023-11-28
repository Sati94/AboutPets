using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using WebShopAPI.Service.OrderServiceMap;
using WebShopAPI.Model.OrderModel;

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
        
    }
}
