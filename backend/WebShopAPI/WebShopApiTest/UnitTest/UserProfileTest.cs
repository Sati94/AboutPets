using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopApiTest.IntegrationTest;

namespace WebShopApiTest.UnitTest
{
    public class UserProfileTest
    {
        private WebShopContext _context;
        private IUserProfileService _userProfileService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;
            _context = new WebShopContext(options);
            SeedData.PopulateTestData(options);


            _userProfileService = new UserProfileService(_context);
        }
        [TearDown]
        public void TearDown()
        {

            CleanUpDate();

        }
        private void CleanUpDate()
        {
            if (_context != null)
            {
                var userProfileDelete = _context.UserProfiles.Where(up => up.FirstName.Contains("Test")).ToList();
                

                _context.UserProfiles.RemoveRange(userProfileDelete);

                _context.SaveChanges();
            }

        }
        //Task<UserProfile> GetUserProfileAsync(string userId);
        //Task<UserProfile> UpdateUserProfile(string userId, UserProfileDto profile);
        //Task<UserProfile> UpdateAdminUserProfileAsync(string userId, AdminUserProfileDto updatedProfile);
        [Test]
        public async Task GetUserProfileById_ShouldReturnIsNotNull()
        {
            var user = await _context.Useres.FirstOrDefaultAsync();
            var userId = user.Id;

            var result = await _userProfileService.GetUserProfileAsync(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
        }
    }
}
