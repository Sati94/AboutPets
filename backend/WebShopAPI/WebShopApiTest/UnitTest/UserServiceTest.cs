using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebShopAPI.Data;
using WebShopApiTest.IntegrationTest;



namespace WebShopApiTest.UnitTest
{


    public class UserServiceTest
    {
        private Mock<WebShopContext> _mockWebShopContext;
        private IUserService _userService;
        private Mock<UserManager<IdentityUser>> _mockUserManager;

        [SetUp]
        public void Setup()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStore.Object, null, null, null, null, null, null, null, null);

            var options = new DbContextOptionsBuilder<WebShopContext>()
                         .UseInMemoryDatabase(databaseName: "TestDataBase")
                         .Options;

            _mockWebShopContext = new Mock<WebShopContext>(options);


            _userService = new UserService(_mockWebShopContext.Object, _mockUserManager.Object);
        }
        
        [Test]
        public async Task GetUserById_ShouldReturnUsers()
        {
            var userId = "someUserId";
            var expectedUser = new IdentityUser { Id = userId, UserName = "testUser" };

            _mockUserManager.Setup(m => m.FindByIdAsync(userId))
                   .ReturnsAsync(expectedUser);
            var result = await _userService.GetUserById(userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(userId));
            Assert.That(result.UserName, Is.EqualTo(expectedUser.UserName));


        }
        [Test]
        public async Task GetUserByName_ShouldReturnUser()
        {
            // Arrange
            var user = new IdentityUser
            { 
                Id = "123456asd",
                UserName = "test2",
                Email = "test2@test.com"
            };
            _mockUserManager.Setup(u => u.FindByNameAsync("test2")).ReturnsAsync(user);
            var userName = user.UserName;

            var result = await _userService.GetUserByName(userName);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Is.EqualTo(userName));
        }
        [Test]
        public async Task UpdateUser_ShouldUpdateUser()
        {
            var user = new IdentityUser
            {
                Id = "123456asd",
                UserName = "test",
                Email = "test@test.com"
            };

            var updatedUsername = "UpdatedUserName";
            var updatedEmail = "updated@test.com";

            _mockUserManager.Setup(u => u.FindByIdAsync("123456asd")).ReturnsAsync(user);
            _mockUserManager.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.UpdateUser("123456asd", new UserDto { Username = updatedUsername, Email = updatedEmail, Password = null });

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Is.EqualTo(updatedUsername));
            Assert.That(result.Email, Is.EqualTo(updatedEmail));
            Assert.That(result.PasswordHash, Is.EqualTo(user.PasswordHash)); 
        }
    }
}
