using WebShopAPI.Model.UserModels;

namespace WebShopAPI.Service.UserServiceMap
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUserAsync();
    }
}
