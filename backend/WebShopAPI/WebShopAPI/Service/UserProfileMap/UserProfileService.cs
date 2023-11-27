using Microsoft.EntityFrameworkCore;
using WebShopAPI.Data;
using WebShopAPI.Model.UserModels;
using WebShopAPI.Model.DTOS;

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

        public async Task<UserProfile> UpdateUserProfile(string userId , UserProfileDto userProfile)
        {
            var existingProfile = await _context.UserProfiles.FirstOrDefaultAsync(p=> p.UserId == userId);
            if (existingProfile == null)
            {
                throw new InvalidOperationException("UserProfile not found");
            }
            existingProfile.FirstName = userProfile.FirstName;
            existingProfile.LastName = userProfile.LastName;
            existingProfile.PhoneNumber = userProfile.PhoneNumber;
            existingProfile.Address = userProfile.Address;

            await _context.SaveChangesAsync();
            return existingProfile;
        }
        public async Task<UserProfile> UpdateAdminUserProfileAsync(string userId, AdminUserProfileDto updatedProfile)
        {
            var existingProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (existingProfile == null)
            {
                throw new InvalidOperationException("UserProfile not found");
            }
            existingProfile.FirstName = updatedProfile.FirstName;
            existingProfile.LastName = updatedProfile.LastName;
            existingProfile.PhoneNumber = updatedProfile.PhoneNumber;
            existingProfile.Address = updatedProfile.Address;
            existingProfile.Bonus = updatedProfile.Bonus;

            await _context.SaveChangesAsync();
            return existingProfile;
        }
    }
    
}
