using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Service.OrderItemServiceMap;
using WebShopAPI.Model;
using WebShopAPI.Data;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace WebShopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;
        private readonly ILogger<OrderItemController> _logger;



        public OrderItemController(IOrderItemService orderItemService, ILogger<OrderItemController> logger)
        {
            _orderItemService = orderItemService;
            _logger = logger;   
        }

        [HttpPost("/add"), Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<OrderItem>> AddOrderItemToUserAsync(string userId, int productId, int quantity, int orderid)
        {
            try
            {
                var result = await _orderItemService.AddOrderItemToUser(userId, productId, quantity, orderid);
               
                return Ok(result);
                
            }
            catch(ArgumentException ex) 
            {
                _logger.LogError(ex, $"Hiba a kódban: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // ILogger beinjektálása a konstruktorban
                _logger.LogError(ex, $"Egyéb hiba a kódban: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }


        }
        [HttpDelete("/orderitem/remove"), Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<OrderItem>> RemoveOrderItemFromUserAndOrderAsync(string userid,int orderItemId)
        {
            try
            {
                await _orderItemService.DeleteOrderItem(userid, orderItemId);
                return Ok("OrderItem removed !");
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            };
        }
        [HttpPut("/orderitem/updateQuantity"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<OrderItem>> SetOrderItemQuantityAsync(string userId, int orderItemId, int newQuantity)
        {
            try
            {
                var updateItem = await _orderItemService.SetOrderItemQuantity(userId, orderItemId, newQuantity);
                if(updateItem != null)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    };
                    return Ok(JsonSerializer.Serialize(updateItem, jsonOptions));
                }
                return NotFound($"OrderItem with id {orderItemId} not found for user {userId}");
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message );
            }
        }
    }
}
