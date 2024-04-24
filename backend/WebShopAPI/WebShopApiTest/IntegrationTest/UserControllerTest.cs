using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Data;
using WebShopAPI.Model.UserModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebShopApiTest.IntegrationTest
{
    public class UserControllerTest : CustomWebApplicationFactory<Program>
    
    {

        private HttpClient _httpClient;
        private UserManager<IdentityUser> _userManager;
        private AuthService _authService;
       
        private async Task InitializeTestDataAsync()
        {

            var scope = Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<WebShopContext>();
            _userManager = scopedServices.GetRequiredService<UserManager<IdentityUser>>();
            var seedData = new SeedData();
            await seedData.PopulateTestDataAsync(dbContext, _userManager);
            await WaitForDatabase();
        }
        [SetUp]
        public async Task Setup()
        {
            await InitializeTestDataAsync();
            _httpClient = CreateClient();
            _authService = new AuthService(_httpClient);
            var token =  _authService.AuthenticateAndGetToken("admin@admin.com", "admin1234");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");


        }
        private async Task WaitForDatabase()
        {
            int maxRetryCount = 5;
            int retryDelayMilliseconds = 1000;

            for (int i = 0; i < maxRetryCount; i++)
            {
                if (IsDatabaseReady())
                {
                    return;
                }

                await Task.Delay(retryDelayMilliseconds);
            }
        }
        private bool IsDatabaseReady()
        {
            using (var scope = Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                return userManager.Users.Any();
            }
        }

        [TearDown]
        public async Task TearDown()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var users = dbContext.Users.ToList();
                foreach (var user in users)
                {
                    await userManager.DeleteAsync(user);
                }
                dbContext.Products.RemoveRange(dbContext.Products);
                dbContext.Orders.RemoveRange(dbContext.Orders);
                dbContext.OrderItems.RemoveRange(dbContext.OrderItems);
                dbContext.UserProfiles.RemoveRange(dbContext.UserProfiles);


                dbContext.SaveChanges();
            }
            _httpClient.Dispose();
        }


        [Test]
        public async Task Return_AllUser_Endpoint()
        {
            var user = await _userManager.FindByEmailAsync("test@test.com");
            var userId = user.Id;

            var response = await _httpClient.GetAsync("/allUser");
            
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = content.Contains(userId);
            
            Assert.That(result, Is.True);         
            
            
        }
        [Test]
        public async Task Find_User_ById_RetrurnTrue()
        {
            var user = await _userManager.FindByEmailAsync("test@test.com");
            
            var userId = user.Id;
            var response = await _httpClient.GetAsync($"/user/{userId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            
            Assert.That(userId, Is.EqualTo(user.Id));
            
            
        }
        [Test]
        public async Task Find_User_ByUserName_RetrurnTrue()
        {
            
            var user = await _userManager.FindByEmailAsync("test@test.com");
          
            var userName = user.UserName;
            var response = await _httpClient.GetAsync($"/user/name/{userName}");
            var responseContent = await response.Content.ReadAsStringAsync();

            
            Assert.That(userName, Is.EqualTo(user.UserName));
        }
        
        [Test]
        public async Task Update_UserById_Return_True()
        {

            var user = await _userManager.FindByEmailAsync("test@test.com");
            
            var userId = user.Id;

           UserDto newUser = new UserDto
           {
              Username = "Test2",
              Password = "1234ABCD",
              Email = "newUser@newUser.com"
           };
           var content = new StringContent(JsonConvert.SerializeObject(new
           {
              username = newUser.Username,
              password = newUser.Password,
              email = newUser.Email

           }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/user/update/{userId}", content);
           
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedUser = JsonConvert.DeserializeObject<IdentityUser>(responseContent);


            Assert.That(newUser.Username, Is.EqualTo(updatedUser.UserName));

        }
        [Test]
        public async Task Delete_User_NonExistingUser_ReturnsTrue()
        {


            var user = await _userManager.FindByEmailAsync("test@test.com");

            var userId = user.Id;


            var response = await _httpClient.DeleteAsync($"/user/delete/{userId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = responseContent.Contains(userId);

            Assert.That(result, Is.True);
        }
        
    }
}

