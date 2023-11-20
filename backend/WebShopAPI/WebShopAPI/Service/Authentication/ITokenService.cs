using Microsoft.AspNetCore.Identity;

namespace WebShopAPI.Service.Authentication
{
    public interface ITokenService
    {
        string CreateToken(IdentityUser user, string role);
    }
}
