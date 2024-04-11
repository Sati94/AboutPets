using Microsoft.AspNetCore.Identity;
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
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            var userManager = new UserManager<IdentityUser>(userStoreMock.Object,null,null, null, null, null, null, null, null);

            _userService = new UserService(_context,userManager );
        }
        [Test]
        public async Task GetUserById_ShouldReturnTrue()
        {
            var user = await _context.Useres.FirstOrDefaultAsync();
            var userId = user.Id;

            var result =  _userService.GetUserById(userId);

            Assert.IsNotNull(result);
            
          
        }
        [Test]
        public async Task GetUserByName_ShouldReturnUser()
        {
            // Arrange
            var user = await _context.Useres.FirstOrDefaultAsync();
            var userName = user.UserName;
           

            // Act
            var result = await _userService.GetUserByName(userName);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(userName, result.UserName);
        }
        [Test]
        public async Task UpdateUser_ShouldUpdateUser()
        {
            // Arrange
            var user = await _context.Useres.FirstOrDefaultAsync();
            var userId = user.Id;
            var updatedUsername = "UpdatedUserName";

            var userDto = new UserDto
            {
                Username = updatedUsername,
                Email = "updated@test.com"
            };

            // Act
            var result = await _userService.UpdateUser(userId, userDto);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(updatedUsername, result.UserName);

            var updatedUserInDb = await _context.Useres.FirstOrDefaultAsync(u => u.Id == userId);
            Assert.NotNull(updatedUserInDb);
            Assert.AreEqual(updatedUsername, updatedUserInDb.UserName);
        }

    }
}
