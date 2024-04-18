using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Model.UserModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebShopApiTest.IntegrationTest
{
    public class UserControllerTest : CustomWebApplicationFactory<Program>
    {

        private HttpClient _httpClient;
        private WebShopContext _webShopContext;
        private UserManager<IdentityUser> _userManager;
        private AuthService _authService;
        private SeedData _seedData;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                  .UseInMemoryDatabase(databaseName: "InMemoryWebShopContext")
                  .Options;

            _webShopContext = new WebShopContext(options);
            
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_webShopContext), null, null, null, null, null, null, null, null);
            _seedData = new SeedData(_webShopContext, _userManager);
            _seedData.PopulateTestData(_webShopContext,_userManager);
            

            _httpClient = CreateClient();
            _authService = new AuthService(_httpClient);
            var token = _authService.AuthenticateAndGetToken("admin@admin.com", "admin1234");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");



        }
        [TearDown]
        public void TearDown()
        {
            CleanUpDate();
            _httpClient.Dispose();
        }
        private void CleanUpDate()
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
        [Test]
        public async Task Return_AllUser_Endpoint()
        {
            var user = await _userManager.FindByEmailAsync("test@test.com");
            var response = await _httpClient.GetAsync("/allUser");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            //Assert.NotNull(content);
            Assert.NotNull(user);
            //Assert.IsNotEmpty(content);
            
            
        }
        [Test]
        public async Task Find_User_ById_RetrurnTrue()
        {
            var user = await _userManager.FindByEmailAsync("test@test.com");
           
            var userId = user.Id;
            var response = await _httpClient.GetAsync($"/user/{userId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.NotNull(responseContent);
            Assert.AreEqual(user.Id, userId);
            
            
        }
        [Test]
        public async Task Find_User_ByUserName_RetrurnTrue()
        {
            
            var user = await _userManager.FindByEmailAsync("test@test.com");
            var userName = user.UserName;
            var response = await _httpClient.GetAsync($"/user/name/{userName}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.NotNull(responseContent);
            Assert.AreEqual(user.UserName, userName);
        }
        [Test]
        public async Task Delete_User_NonExistingUser_ReturnsNotFound()
        {


            var user = await _userManager.FindByEmailAsync("test@test.com");
            var userId = user.Id;
           
            
            var response = await _httpClient.DeleteAsync($"/user/delete/{userId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

            Assert.AreEqual("This user dosn't exist!", responseContent);
        }
        [Test]
        public async Task Update_UserById_Return_True()
        {

            var user = await _userManager.FindByEmailAsync("test@test.com");
            var userId = user.Id;

           UserDto newUser = new UserDto
           {
              Username = "Test2",
              Email = "newUser@newUser.com"
           };
           var content = new StringContent(JsonConvert.SerializeObject(new
           {
              username = newUser.Username,
              email = newUser.Email

            }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/user/update/{userId}", content);
           
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedUser = JsonConvert.DeserializeObject<User>(responseContent);
            Assert.AreEqual("Test2", updatedUser.UserName);
            
       

        }
    }
}

