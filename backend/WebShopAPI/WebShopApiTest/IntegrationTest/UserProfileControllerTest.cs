using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Model.UserModels;

namespace WebShopApiTest.IntegrationTest
{
    public class UserProfileControllerTest : WebApplicationFactory<Program>
    {
        private HttpClient _httpClient;
        private WebShopContext _webShopContext;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                 .UseInMemoryDatabase(databaseName: "InMemoryWebShopContext")
                 .Options;

            _webShopContext = new WebShopContext(options);
            SeedData.PopulateTestData(options);

            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            _httpClient = CreateClient();
            AuthRequest authRequest = new AuthRequest("admin@admin.com", "admin1234");
            string jsonString = JsonSerializer.Serialize(authRequest);
            StringContent jsonStringContent = new StringContent(jsonString);
            jsonStringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = _httpClient.PostAsync("/Login", jsonStringContent).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var desContent = JsonSerializer.Deserialize<AuthResponse>(content, option);
            var token = desContent.Token;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
        [OneTimeTearDown]
        public void TearDown()
        {
            CleanUpDate();
            _httpClient.Dispose();
        }
        private void CleanUpDate()
        {
            

            var productsToDelete = _webShopContext.Products.Where(p => p.ProductName.Contains("Test")).ToList();
            var userDelete = _webShopContext.Useres.Where(u => u.UserName.Contains("Test")).ToList();
            var orderToDelete = _webShopContext.Orders.Where(o => o.User.UserName.Contains("Test")).ToList();
            var orderItemDelete = _webShopContext.OrderItems.Where(oi => oi.User.UserName == "Test").ToList();
            var userProfileToDelete = _webShopContext.UserProfiles.Where(up => up.User.UserName.Contains("Test")).ToList();

            _webShopContext.Products.RemoveRange(productsToDelete);
            _webShopContext.Useres.RemoveRange(userDelete);
            _webShopContext.Orders.RemoveRange(orderToDelete);
            _webShopContext.OrderItems.RemoveRange(orderItemDelete);
            _webShopContext.UserProfiles.RemoveRange(userProfileToDelete);
            _webShopContext.SaveChanges();
        }
        [Test]
        public async Task GetUserProfile_ByUserId_Return_True()
        {
            string userId = "abcde";
            var userProfile = _webShopContext.UserProfiles.FirstOrDefault(up => up.UserId == userId);
            var response = await _httpClient.GetAsync($"/user/profile/{userId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseContent);
            Assert.AreEqual(userProfile.UserId, userId);
        }
        [Test]
        public async Task Update_UserProfile_ByAdmin_Return_True()
        {
            string userId = "abcde";
            var userProfile = _webShopContext.UserProfiles.FirstOrDefault(up => up.UserId == userId);
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

            Assert.AreEqual(updater.FirstName, updatedProfile.FirstName);
            Assert.AreEqual(updater.LastName, updatedProfile.LastName);
            Assert.AreEqual(updater.PhoneNumber, updatedProfile.PhoneNumber);
            Assert.AreEqual(updater.Address, updatedProfile.Address);
            Assert.AreEqual(updater.Bonus, updatedProfile.Bonus);
        }
    }
}
