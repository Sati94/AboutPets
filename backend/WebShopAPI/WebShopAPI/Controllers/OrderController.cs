using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using WebShopAPI.Service.OrderServiceMap;
using WebShopAPI.Model.OrderModel;
using WebShopAPI.Model.OrderModel.OrderStatus;
using WebShopAPI.Model;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using WebShopAPI.Model.OrderModel.DeliveryType;

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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("/order/pending/{userId}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<Order>> GetOrderByIdPending(string userId)
        {
            try
            {
                var order = await _orderService.GetPendingOrders(userId);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }
        [HttpGet("/order/orderItems/{orderId}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(int orderId)
        {
            var order = await _orderService.GetOrderItemsByOrderIdAsync(orderId);

            if (order == null || !order.OrderItems.Any())
            {
                return NotFound(new { message = "No itmes found for this order!" });
            }
            return Ok(order.OrderItems);
        }
        [HttpGet("/order/{orderId}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<Order>> GetOrderById(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("/order/user/{userId}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrderByUserId(string userId)
        {
            try
            {
                var order = await _orderService.GetOrderByUserId(userId);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("/order/delete/{orderId}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<Order>> DeleteOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _orderService.DeleteOrderById(orderId);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("/order/update/{orderId}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<bool>> UpdateStatusByOrderId(int orderId, [FromBody] int orderStatuses)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatus(orderId, orderStatuses);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("/order/{orderId}/apply-cupon/{userId}"), Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<bool>> UpdateBonusInOrderById(int orderId, string userId)
        {
            try
            {
                var result = await _orderService.UpdateOrderTotlaPriceWithBonus(orderId, userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("/update-order-delivery/{orderId}")]
        public async Task<ActionResult<bool>> UpdateOrderDelivery(int orderId, [FromBody] UpdateOrderDeliveryRequest request)
        {
            var updateResult = await _orderService.UpdateOrderDeliveryTypeAndAddress(orderId, request);

            if (updateResult)
            {
                return Ok("Order delivery type and address updated successfully.");
            }
            else
            {
                return BadRequest("Failed to update order delivery type and address.");
            }

        }
    }
}
