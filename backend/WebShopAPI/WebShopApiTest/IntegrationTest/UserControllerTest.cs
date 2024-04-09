using Microsoft.AspNetCore.Identity;
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
        /*[OneTimeTearDown]
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
        }*/
        [Test]
        public async Task Return_AllUser_Endpoint()
        {
            var response = await _httpClient.GetAsync("/allUser");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.IsNotEmpty(content);
        }
        [Test]
        public async Task Find_User_ById_RetrurnTrue()
        {
            var user = _webShopContext.Useres.FirstOrDefault(u => u.UserName == "Test");
           
                var userId = user.Id;
                var response = await _httpClient.GetAsync($"/user/{userId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                Assert.NotNull(responseContent);
                Assert.AreEqual(user.Id, userId);
            
            
        }
        [Test]
        public async Task Find_User_ByUserName_RetrurnTrue()
        {
            string userName = "Test";
            var user = _webShopContext.Useres.FirstOrDefault(u => u.UserName == userName);

            var response = await _httpClient.GetAsync($"/user/name/{userName}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.NotNull(responseContent);
            Assert.AreEqual(user.UserName, userName);
        }
        [Test]
        public async Task Delete_User_NonExistingUser_ReturnsNotFound()
        {
            
            var user = await _webShopContext.Useres.FirstOrDefaultAsync();
            var userId =  user.IdentityUserId;
            
            var response = await _httpClient.DeleteAsync($"/user/delete/{userId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

            Assert.AreEqual("This user dosn't exist!", responseContent);
        }
        [Test]
        public async Task Update_UserById_Return_True()
        {
           
            var user = await _webShopContext.Useres.FirstOrDefaultAsync();
            var userId = user.IdentityUserId;

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
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // Ha NotFound válasz érkezik, akkor valószínűleg probléma van a teszt környezettel vagy a beállításokkal
                // Helyette kiírhatunk egy hibaüzenetet, és megállíthatjuk a tesztet
                Console.WriteLine("A felhasználó frissítése nem sikerült, mert a felhasználó nem található.");
                Assert.Fail();
            }
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedUser = JsonConvert.DeserializeObject<User>(responseContent);
            Assert.AreEqual("Test2", updatedUser.UserName);
            
       

        }
    }
}

