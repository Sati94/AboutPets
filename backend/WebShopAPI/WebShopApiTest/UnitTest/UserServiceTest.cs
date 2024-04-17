using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Data;
using WebShopAPI.Data;
using WebShopApiTest.IntegrationTest;



namespace WebShopApiTest.UnitTest
{


    public class UserServiceTest
    {
        private WebShopContext _webShopContext;
        private IUserService _userService;
        private UserManager<IdentityUser> _userManager;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _webShopContext = new WebShopContext(options);
            SeedData.PopulateTestData(options);

             _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_webShopContext), null, null, null, null, null, null, null, null);

            _userService = new UserService(_webShopContext, _userManager);
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
        public async Task GetUserById_ShouldReturnTrue()
        {
            var user = await _webShopContext.Users.FirstOrDefaultAsync();
            var userId = user.Id;

            var result = await _userService.GetUserById(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.Id);


        }
        [Test]
        public async Task GetUserByName_ShouldReturnUser()
        {
            // Arrange
            var user = await _userManager.FindByEmailAsync("test@test.com");
            var userName = user.UserName;

            var result = await _userService.GetUserByName(userName);

            Assert.IsNotNull(result);
            Assert.AreEqual(userName, result.UserName);
        }
        [Test]
        public async Task UpdateUser_ShouldUpdateUser()
        {
            var user = await _webShopContext.Users.FirstOrDefaultAsync();
            var userId = user.Id;
            var updatedUsername = "UpdatedUserName";

            var userDto = new UserDto
            {
                Username = updatedUsername,
                Email = "updated@test.com"
            };

            var result = await _userService.UpdateUser(userId, userDto);

            Assert.IsNotNull(result);
            Assert.AreEqual(updatedUsername, result.UserName);

            var updatedUserInDb = await _webShopContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            Assert.IsNotNull(updatedUserInDb);
            Assert.AreEqual(updatedUsername, updatedUserInDb.UserName);

        }
    }
}
