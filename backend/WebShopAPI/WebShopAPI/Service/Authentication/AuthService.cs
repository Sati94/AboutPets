using Microsoft.AspNetCore.Identity;
using WebShopAPI.Data;
using WebShopAPI.Model.UserModels;

namespace WebShopAPI.Service.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly WebShopContext _dataContext;
        public AuthService(UserManager<IdentityUser> userManager, ITokenService tokenService, WebShopContext dataContext)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _dataContext = dataContext;
        }
        public async Task<AuthResult> RegisterAsync(string email, string username, string password, string role)
        {
            var identityuser = new IdentityUser { UserName = username, Email = email };

            var result = await _userManager.CreateAsync(identityuser, password);

            if (!result.Succeeded)
            {
                return FailedRegistration(result, email, username);
            }
            await _userManager.AddToRoleAsync(identityuser, role);

            _dataContext.Users.Add(new User { UserName = username, Email = email, UserId = int.Parse(identityuser.Id) });

            await _dataContext.SaveChangesAsync();
            return new AuthResult(true, identityuser.Id, email, username, "");
        }
        private static AuthResult FailedRegistration(IdentityResult result, string email, string username)
        {
            var authResult = new AuthResult(false, null, email, username, "");

            foreach (var error in result.Errors)
            {
                authResult.ErrorMessages.Add(error.Code, error.Description);
            }

            return authResult;
        }
        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var managedUser = await _userManager.FindByEmailAsync(email);

            if (managedUser == null)
            {
                return InvalidEmail(email);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, password);
            if (!isPasswordValid)
            {
                return InvalidPassword(email, managedUser.UserName);
            }
            var roles = await _userManager.GetRolesAsync(managedUser);
            var role = roles.First();
            Console.WriteLine(role);
            var adminAccessToken = _tokenService.CreateToken(managedUser, role);


            return new AuthResult(true, managedUser.Id, managedUser.Email, managedUser.UserName, adminAccessToken);

        }
        private static AuthResult InvalidEmail(string email)
        {
            var result = new AuthResult(false, null, email, "", "");
            result.ErrorMessages.Add("Bad credentials", "Invalid email");
            return result;
        }
        private static AuthResult InvalidPassword(string email, string userName)
        {
            var result = new AuthResult(false, null, email, userName, "");
            result.ErrorMessages.Add("Bad credentials", "Invalid password");
            return result;
        }
    }
}
