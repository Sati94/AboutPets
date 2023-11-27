using Microsoft.EntityFrameworkCore;
using WebShopAPI.Data;
using WebShopAPI.Model.UserModels;

namespace WebShopAPI.Service.UserProfileMap
{
    public class UserProfileService : IUserProfileService
    {
        private readonly WebShopContext _context;
        public UserProfileService(WebShopContext context)
        {
            _context = context;
        }

        public async Task<UserProfile> GetUserProfileAsync(string userId)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
}
