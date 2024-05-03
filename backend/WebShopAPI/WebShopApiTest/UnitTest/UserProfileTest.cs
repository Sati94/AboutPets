using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Data;
using WebShopApiTest.IntegrationTest;

namespace WebShopApiTest.UnitTest
{
    public class UserProfileTest
    {
        private WebShopContext _webShopContext;
        private IUserProfileService _userProfileService;
        private Mock<UserManager<IdentityUser>> _mockUserManager;

        [SetUp]
        public void SetUp()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var options = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;
            _webShopContext = new WebShopContext(options);
           

            _userProfileService = new UserProfileService(_webShopContext);
        }
        [TearDown]
        public void TearDown()
        {

            _webShopContext.Dispose();

        }
       

        
        
        [Test]
        public async Task GetUserProfileById_ShouldReturnIsNotNull()
        {
            var userId = "someUserId";
            var profile = new UserProfile { UserId = userId, FirstName = "Test", LastName = "Test" , Address = "Test", PhoneNumber = "00"};
            _webShopContext.UserProfiles.Add(profile);
            await _webShopContext.SaveChangesAsync();

            var result = await _userProfileService.GetUserProfileAsync(userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(userId,Is.EqualTo( result.UserId));
        }
        [Test]
        public async Task UpdateUserProfile_ShouldReturnNotNull()
        {
            var userId = "someUserId";

            var newUserProfile = new UserProfileDto
            {
                FirstName = "Test2",
                LastName = "Test",
                PhoneNumber = "1234567890",
                Address = "SomeWhere"

            };

            var userProfile = new UserProfile
            {
                FirstName = newUserProfile.FirstName,
                LastName = newUserProfile.LastName,
                PhoneNumber = newUserProfile.PhoneNumber,
                Address = newUserProfile.Address

            };
            var result = await _userProfileService.UpdateUserProfile(userId, newUserProfile);

            Assert.That(result, Is.Not.Null);
            Assert.That(userId, Is.EqualTo(result.UserId));
            Assert.That(userProfile.LastName, Is.EqualTo(result.LastName));

        }
        [Test]
        public async Task UpdatedAdminUserProfile_ShouldReturnNotNull()
        {
            var userId = "someUserId";

            var newUserProfile = new AdminUserProfileDto
            {
                FirstName = "Test2",
                LastName = "Test",
                PhoneNumber = "1234567890",
                Address = "SomeWhere",
                Bonus = 10
             
            };

            var userProfile = new UserProfile
            {
                FirstName = newUserProfile.FirstName,
                LastName = newUserProfile.LastName,
                PhoneNumber = newUserProfile.PhoneNumber,
                Address = newUserProfile.Address,
                Bonus = newUserProfile.Bonus
                

            };
            var result = await _userProfileService.UpdateAdminUserProfileAsync(userId, newUserProfile);

            Assert.That(result, Is.Not.Null);
            Assert.That(userId, Is.EqualTo(result.UserId));
            Assert.That(userProfile.Bonus, Is.EqualTo(result.Bonus));
        }
    }
}
