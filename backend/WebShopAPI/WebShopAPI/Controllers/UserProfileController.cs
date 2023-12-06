using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Model.UserModels;
using WebShopAPI.Service.UserProfileMap;
using WebShopAPI.Service.UserServiceMap;
using WebShopAPI.Model.DTOS;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebShopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserProfileController : ControllerBase
    {
       
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }
        [HttpGet("user/profile/{userId}"), Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<UserProfile>> GetUserProfileByIdAsync(string userId)
        {
            var userProfile = await _userProfileService.GetUserProfileAsync(userId);
            if(userProfile == null)
            {
                return NotFound("This Profile: {userid} not found!");
            }
            return Ok(userProfile);
        }
        [HttpPut("/update/user/profile/{userId}"), Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<UserProfile>> UpdateUserProfileAsync(string userId,[FromBody] UserProfileDto userProfile)
        {
            try
            {
                var profile = await _userProfileService.UpdateUserProfile(userId,userProfile);
                return Ok(profile);
            }
            catch(InvalidOperationException ex) 
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("/admin/update/profile/{userId}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserProfile>> UpdateAdminUserProfile(string userId, [FromBody] AdminUserProfileDto userProfile)
        {
            try
            {
                var updatedProfile = await _userProfileService.UpdateAdminUserProfileAsync(userId, userProfile);
                return Ok(updatedProfile);
            }
            catch(InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
    }
}
