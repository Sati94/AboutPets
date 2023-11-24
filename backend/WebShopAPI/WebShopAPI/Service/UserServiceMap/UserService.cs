using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebShopAPI.Data;
using WebShopAPI.Model.UserModels;

namespace WebShopAPI.Service.UserServiceMap
{
    public class UserService : IUserService
    {
        private readonly WebShopContext _context;
        private readonly UserManager<User> _userManager;
        public UserService(WebShopContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

       public async Task<IEnumerable<User>> GetAllUserAsync()
        {
            var allUser = await _userManager.Users.ToListAsync();
            return allUser;
        }
    }
}
