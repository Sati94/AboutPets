using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Service.OrderItemServiceMap;
using WebShopAPI.Model;
using WebShopAPI.Data;

namespace WebShopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;
       

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
          
        }

        [HttpPost("/add"), Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<OrderItem>> AddOrderItemToUserAsync(string userId, int productId, int quantity, int orderid)
        {
            try
            {
                await _orderItemService.AddOrderItemToUser(userId, productId, quantity, orderid);
                return Ok("OrderItem added to user and database");
            }
            catch(ArgumentException ex) 
            {
                return BadRequest(ex.Message);
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
    }
}
