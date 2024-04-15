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
                var productsToDelete = _context.Products.Where(p => p.ProductName.Contains("Test")).ToList();
                var userDelete = _context.Useres.Where(u => u.UserName.Contains("Test")).ToList();
                var orderToDelete = _context.Orders.Where(o => o.User.UserName.Contains("Test")).ToList();
                var orderItemDelete = _context.OrderItems.Where(oi => oi.User.UserName == "Test").ToList();
                var userProfileToDelete = _context.UserProfiles.Where(up => up.User.UserName.Contains("Test")).ToList();

                _context.Products.RemoveRange(productsToDelete);
                _context.Useres.RemoveRange(userDelete);
                _context.Orders.RemoveRange(orderToDelete);
                _context.OrderItems.RemoveRange(orderItemDelete);
                _context.UserProfiles.RemoveRange(userProfileToDelete);

                _context.SaveChanges();
            }

        }
        
        [Test]
        public async Task GetUserProfileById_ShouldReturnIsNotNull()
        {
            var user = await _context.Useres.FirstOrDefaultAsync();
            var userId = user.Id;

            var result = await _userProfileService.GetUserProfileAsync(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
        }
        [Test]
        public async Task UpdateUserProfile_ShouldReturnNotNull()
        {
            var user = await _context.Useres.FirstOrDefaultAsync();
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
            var user = await _context.Useres.FirstOrDefaultAsync();
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
