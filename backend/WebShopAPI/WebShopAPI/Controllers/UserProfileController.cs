﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Model.UserModels;
using WebShopAPI.Service.UserProfileMap;
using WebShopAPI.Service.UserServiceMap;

namespace WebShopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserService userService, IUserProfileService userProfileService)
        {
            _userService = userService;
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
        
    }
}
