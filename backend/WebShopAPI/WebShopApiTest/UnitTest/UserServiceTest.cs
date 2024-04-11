using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Data;
using WebShopApiTest.IntegrationTest;



namespace WebShopApiTest.UnitTest
{


    public class UserServiceTest
    {
        private WebShopContext _context;
        private IUserService _userService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new WebShopContext(options);
            SeedData.PopulateTestData(options);

            var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_context), null, null, null, null, null, null, null, null);

            _userService = new UserService(_context, userManager);
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
                var userDelete = _context.Useres.Where(u => u.UserName.Contains("Test")).ToList();
                var identityUserDelete = _context.Users.Where(u => u.UserName.Contains("Test")).ToList();


                _context.Useres.RemoveRange(userDelete);
                _context.Users.RemoveRange(identityUserDelete);

                _context.SaveChanges();
            }

        }
        [Test]
        public async Task GetUserById_ShouldReturnTrue()
        {
            var user = await _context.Users.FirstOrDefaultAsync();
            var userId = user.Id;

            var result = await _userService.GetUserById(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.Id);


        }
        [Test]
        public async Task GetUserByName_ShouldReturnUser()
        {
            // Arrange
            var user = await _context.Users.FirstOrDefaultAsync();
            var userName = user.UserName;

            var result = await _userService.GetUserByName(userName);

            Assert.IsNotNull(result);
            Assert.AreEqual(userName, result.UserName);
        }
        [Test]
        public async Task UpdateUser_ShouldUpdateUser()
        {
            var user = await _context.Users.FirstOrDefaultAsync();
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

            var updatedUserInDb = await _context.Useres.FirstOrDefaultAsync(u => u.Id == userId);
            Assert.IsNotNull(updatedUserInDb);
            Assert.AreEqual(updatedUsername, updatedUserInDb.UserName);

        }
    }
}
