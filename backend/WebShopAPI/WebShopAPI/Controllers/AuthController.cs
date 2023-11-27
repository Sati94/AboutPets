using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Contracts;
using WebShopAPI.Data;
using WebShopAPI.Model.UserModels;
using WebShopAPI.Service.Authentication;

namespace WebShopAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly WebShopContext _webShopcontext;
        

        public AuthController(IAuthService authService, WebShopContext webShopcontext)
        {
            _authService = authService;
            _webShopcontext = webShopcontext;
            
        }
        [HttpPost("/Register")]
        public async Task<ActionResult<RegistrationResponse>> Register([FromBody]RegistrationRequest registrationRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(registrationRequest.Email, registrationRequest.UserName, registrationRequest.Password,"User");

            if (!result.Success)
            {
                AddErrors(result);
                return BadRequest(ModelState);
            }
            var resultemail = result.Email;
            var user =  _webShopcontext.Useres.FirstOrDefault(u => u.Email == resultemail);
            if(user != null)
            {
               var userProfile = new UserProfile
               {
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    PhoneNumber = string.Empty,
                    Address = string.Empty,
                    Bonus = decimal.Zero,
                    UserId = user.Id,

               };
               _webShopcontext.UserProfiles.Add(userProfile);
               await _webShopcontext.SaveChangesAsync();
            }
            
            return CreatedAtAction(nameof(Register), new RegistrationResponse(result.Email, result.UserName));
        }
        private void AddErrors(AuthResult result)
        {
            foreach (var error in result.ErrorMessages)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }
        [HttpPost("/Login")]
        public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.LoginAsync(request.Email, request.Password);
            if (!result.Success)
            {
                AddErrors(result);
                return BadRequest(ModelState);
            }
            return Ok(new AuthResponse(result.IdentityUserId, result.Email, result.UserName, result.Token));
        }
    }


}

