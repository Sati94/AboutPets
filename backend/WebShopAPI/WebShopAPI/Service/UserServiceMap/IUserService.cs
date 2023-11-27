using WebShopAPI.Model.UserModels;
using Microsoft.AspNetCore.Identity;
using WebShopAPI.Model.DTOS;


namespace WebShopAPI.Service.UserServiceMap
{
    public interface IUserService
    {
        Task<IEnumerable<IdentityUser>> GetAllUserAsync();
        Task<IdentityUser> UpdateUser(string userId, UserDto user);
        Task<IdentityUser> GetUserById(string userId);
        Task<IdentityUser> GetUserByName(string userName);
        Task<IdentityUser> DeleteUserById(string userId);
    }
}
