using WebShopAPI.Model.UserModels;

namespace WebShopAPI.Service.UserProfileMap
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetUserProfileAsync(string userId);
    }
}
