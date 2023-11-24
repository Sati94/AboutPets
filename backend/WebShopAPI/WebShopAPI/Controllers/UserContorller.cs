using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Data;
using WebShopAPI.Model.UserModels;
using WebShopAPI.Service.UserServiceMap;

namespace WebShopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserContorller : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly WebShopContext _context;
        private readonly ILogger _logger;
        
        public UserContorller(IUserService userService, WebShopContext context, ILogger logger)
        {
            _userService = userService;
            _context = context;
            _logger = logger;
        }
        [HttpGet("/allUser"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUserAsync()
        {
            var useres = await _userService.GetAllUserAsync();
            if(!useres.Any())
            {
                return NotFound("It isn't have any user!");
            }
            return Ok(useres);
        }
    }
}
