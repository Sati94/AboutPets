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

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;
            _webShopContext = new WebShopContext(options);
            SeedData.PopulateTestData(options);


            _userProfileService = new UserProfileService(_webShopContext);
        }
        [TearDown]
        public void TearDown()
        {

            CleanUpDate();

        }
        private void CleanUpDate()
        {
            if (_webShopContext != null)
            {
                var productsToDelete = _webShopContext.Products.Where(p => p.ProductName.Contains("Test")).ToList();
                var productsToDelete2 = _webShopContext.Products.Where(p => p.ProductName.Contains("Test2")).ToList();
                var userDelete = _webShopContext.Users.Where(u => u.UserName.Contains("Test")).ToList();
                var orderToDelete = _webShopContext.Orders.Where(o => o.User.UserName.Contains("Test")).ToList();
                var orderItemDelete = _webShopContext.OrderItems;
                var userProfileToDelete = _webShopContext.UserProfiles.Where(up => up.User.UserName.Contains("Test")).ToList();

                _webShopContext.Products.RemoveRange(productsToDelete);
                _webShopContext.Products.RemoveRange(productsToDelete2);
                _webShopContext.Users.RemoveRange(userDelete);
                _webShopContext.Orders.RemoveRange(orderToDelete);

                _webShopContext.UserProfiles.RemoveRange(userProfileToDelete);
                _webShopContext.SaveChanges();
            }

        }
        
        [Test]
        public async Task GetUserProfileById_ShouldReturnIsNotNull()
        {
            var user = await _webShopContext.Users.FirstOrDefaultAsync();
            var userId = user.Id;

            var result = await _userProfileService.GetUserProfileAsync(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
        }
        [Test]
        public async Task UpdateUserProfile_ShouldReturnNotNull()
        {
            var user = await _webShopContext.Users.FirstOrDefaultAsync();
            var userId = user.Id;

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

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);

        }
        [Test]
        public async Task UpdatedAdminUserProfile_ShouldReturnNotNull()
        {
            var user = await _webShopContext.Users.FirstOrDefaultAsync();
            var userId = user.Id;

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

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
            Assert.AreEqual(userProfile.Bonus, result.Bonus);
        }
    }
}
