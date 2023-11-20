using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Contracts;
using WebShopAPI.Data;
using WebShopAPI.Service.Authentication;

namespace WebShopAPI.Controllers
{
    public class AuthContorller : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly WebShopContext _webShopcontext;

        public AuthContorller(IAuthService authService, WebShopContext webShopcontext)
        {
            _authService = authService;
            _webShopcontext = webShopcontext;
        }
        [HttpPost("Register")]
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
            return CreatedAtAction(nameof(Register), new RegistrationResponse(result.Email, result.UserName));
        }
        private void AddErrors(AuthResult result)
        {
            foreach (var error in result.ErrorMessages)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }
        [HttpPost("Login")]
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
            return Ok(new AuthResponse(int.Parse(result.UserId), result.Email, result.UserName, result.Token));
        }
    }


}
}
