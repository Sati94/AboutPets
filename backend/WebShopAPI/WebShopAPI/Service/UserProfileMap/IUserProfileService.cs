using WebShopAPI.Model.UserModels;
using WebShopAPI.Model.DTOS;

namespace WebShopAPI.Service.UserProfileMap
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetUserProfileAsync(string userId);
        Task<UserProfile> UpdateUserProfile(string userId,UserProfileDto profile);
        Task<UserProfile> UpdateAdminUserProfileAsync(string userId, AdminUserProfileDto updatedProfile);
    }
}
