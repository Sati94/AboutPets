using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebShopAPI.Data;
using WebShopAPI.Model.DTOS;
using WebShopAPI.Model.UserModels;

namespace WebShopAPI.Service.UserServiceMap
{
    public class UserService : IUserService
    {
        private readonly WebShopContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public UserService(WebShopContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<IdentityUser>> GetAllUserAsync()
        {
            var allUser = await _userManager.Users.ToListAsync();
            return allUser;
        }
        public async Task<IdentityUser> UpdateUser(string userId, UserDto user) 
        {
            var newUser = await _userManager.FindByIdAsync(userId);
            var dbUser = await _context.Useres.FirstOrDefaultAsync(user => user.IdentityUserId == userId);
            if(newUser == null)
            {
                return null;
            }
            newUser.UserName = user.Username;
            newUser.Email = user.Email;
            
            dbUser.UserName = newUser.UserName;
            dbUser.Email = newUser.Email;

            var result = await _userManager.UpdateAsync(newUser);
            _context.Useres.Update(dbUser);
            _context.SaveChanges();
            if(result.Succeeded)
            {
                return newUser;
            }
            else
            {
                return null;
            }
        }
    }
}
