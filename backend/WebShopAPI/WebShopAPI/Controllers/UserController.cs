using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Data;
using WebShopAPI.Model.DTOS;
using WebShopAPI.Model.UserModels;
using WebShopAPI.Service.UserServiceMap;

namespace WebShopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
      
        public UserController(IUserService userService)
        {
            _userService = userService;
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
        [HttpPut("/user/update/{userId}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<User>> UpdateUserAsync(string userId, UserDto user)
        {
            var result = await _userService.UpdateUser(userId, user);
            if(result == null)
            {
                return NotFound("This user dosn't exist!");
            }
            return Ok(result);  
        }
        [HttpGet("/user/{userId}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> GetUserByIdAsync(string userId)
        {
            var result = await _userService.GetUserById(userId);
            if(result == null)
            {
                return NotFound("This user dosn't exist!");
            }
            return Ok(result);
        }
        [HttpGet("/user/name/{username}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> GetUserByNameAsync(string username)
        {
            var result = await _userService.GetUserByName(username);
            if(result == null)
            {
                return NotFound("This user dosn't exist!");
            }
            return Ok(result);
        }
        [HttpDelete("/user/delete/{userId}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> DeleteUserByIdAsync(string userId)
        {
            var result = await _userService.DeleteUserById(userId);
            if( result == null)
            {
                return NotFound("This user dosn't exist!");
            }
            return Ok(result);
        }
    }
}
