using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Model.UserModels;

namespace WebShopApiTest.IntegrationTest
{
    public class UserProfileControllerTest : CustomWebApplicationFactory<Program>
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
        [OneTimeSetUp]
        public async Task Setup()
        {
            await InitializeTestDataAsync();
            _httpClient = CreateClient();
            _authService = new AuthService(_httpClient);
            var token = _authService.AuthenticateAndGetToken("admin@admin.com", "admin1234");
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
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                if ( userManager.Users.Any() && dbContext.Products.Any() && dbContext.Orders.Any() && dbContext.OrderItems.Any() && dbContext.UserProfiles.Any())
                {
                    return true;
                }
                return false;
            }
        }

        [OneTimeTearDown]
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
        public async Task GetUserProfile_ByUserId_Return_True()
        {
            
            using (var scope = Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var dbContext = serviceProvider.GetRequiredService<WebShopContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var user = await _userManager.FindByEmailAsync("test@test.com");

                var userId = user.Id;
                var userProfile = await dbContext.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);

                var response = await _httpClient.GetAsync($"/user/profile/{userId}");

                var responseContent = await response.Content.ReadAsStringAsync();

                Assert.That(userId, Is.EqualTo(userProfile.UserId));
               
            }

        }
                
        
        [Test]
        public async Task Update_UserProfile_ByAdmin_Return_True()
        {
            
            using (var scope = Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var dbContext = serviceProvider.GetRequiredService<WebShopContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var user = await _userManager.FindByEmailAsync("test@test.com");

                var userId = user.Id;
                var userProfile = await dbContext.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);
                AdminUserProfileDto updater = new AdminUserProfileDto
                {
                    FirstName = "Nagy",
                    LastName = "Béla",
                    PhoneNumber = "123456789",
                    Address = "Nagy Street 25",
                    Bonus = 0.1m
                };

                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    firstname = updater.FirstName,
                    lastname = updater.LastName,
                    phonenumber = updater.PhoneNumber,
                    address = updater.Address,
                    bonus = updater.Bonus
                }), Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/admin/update/profile/{userId}", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                var updatedProfile = JsonConvert.DeserializeObject<UserProfile>(responseContent);

                Assert.That(updatedProfile.FirstName, Is.EqualTo(updater.FirstName));
                Assert.That(updatedProfile.LastName, Is.EqualTo(updater.LastName));
                Assert.That(updatedProfile.PhoneNumber, Is.EqualTo(updater.PhoneNumber));
                Assert.That(updatedProfile.Address, Is.EqualTo(updater.Address));
                Assert.That(updatedProfile.Bonus, Is.EqualTo(updater.Bonus));


            }
           
           
          
        }
    }
}
