using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Data;
using WebShopAPI.Service.Authentication;

namespace WebShopAPI.Controllers
{
    public class AuthContorller : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly WebShopContext _webShopcontext;

  
    }
}
