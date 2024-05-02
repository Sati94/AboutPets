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
            if (newUser == null)
            {
                return null;
            }
            if(user.Password != null)
            {
                var newPasswordHash = _userManager.PasswordHasher.HashPassword(newUser, user.Password);

                newUser.UserName = user.Username;
                newUser.PasswordHash = newPasswordHash;
                newUser.Email = user.Email;

                var result = await _userManager.UpdateAsync(newUser);

                await _context.SaveChangesAsync();

                return newUser;

            }
            else
            {
                newUser.UserName = user.Username;
               
                newUser.Email = user.Email;

                var result = await _userManager.UpdateAsync(newUser);

                await _context.SaveChangesAsync();

                return newUser;
            }

            

            

        }
        public async Task<IdentityUser> GetUserById(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if(user == null)
            {
                return null;
            }
            return user;
        }
        public async Task<IdentityUser> GetUserByName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            
            if(user == null)
            {
                return null;
            }
            return user;
        }
        public async Task<IdentityUser> DeleteUserById(string userId)
        {
            var userToDelete = await _userManager.FindByIdAsync(userId);
         
            
            await _userManager.DeleteAsync(userToDelete);
            
            _context.SaveChanges();

            return userToDelete;

        }
    }
}
