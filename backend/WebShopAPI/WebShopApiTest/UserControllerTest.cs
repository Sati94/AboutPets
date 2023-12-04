using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Model.UserModels;

namespace WebShopApiTest
{
    public class UserControllerTest : WebApplicationFactory<Program>
    {

        private HttpClient _httpClient;
        private IAuthService _authService;
        private WebShopContext _webShopContext;
        [SetUp]
        public void Setup()
        {
            string connection = "Server=localhost,1433;Database=PetProject;User Id=sa;Password=SaraAttila1994;Encrypt=True;TrustServerCertificate=True;";
            Environment.SetEnvironmentVariable("CONNECTION_STRING", connection);
            var dbConnection = new DbContextOptionsBuilder<WebShopContext>()
            .UseSqlServer(connection)
                .Options;
            _webShopContext = new WebShopContext(dbConnection);
            var options = new JsonSerializerOptions
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
            var desContent = JsonSerializer.Deserialize<AuthResponse>(content, options);
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
            var options = new DbContextOptionsBuilder<WebShopContext>()
                 .UseInMemoryDatabase(databaseName: "TestDatabase")
                 .Options;

            var productsToDelete = _webShopContext.Products.Where(p => p.ProductName.Contains("Test")).ToList();

            _webShopContext.Products.RemoveRange(productsToDelete);
            _webShopContext.SaveChanges();
        }
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
            string userId = "02abd951-63df-432d-a573-ba8d649c33bc";
             var user = _webShopContext.Useres.FirstOrDefault(u => u.IdentityUserId == userId);

            var response = await _httpClient.GetAsync($"/user/{userId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.NotNull(responseContent);
            Assert.AreEqual(user.IdentityUserId, userId);
        }
        [Test]
        public async Task Find_User_ByUserName_RetrurnTrue()
        {
            string userName = "admin";
            var user = _webShopContext.Useres.FirstOrDefault(u => u.UserName == userName);

            var response = await _httpClient.GetAsync($"/user/name/{userName}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.NotNull(responseContent);
            Assert.AreEqual(user.UserName, userName);
        }
        [Test]
        public async Task  Delete_User_NonExistingUser_ReturnsNotFound()
        {
            var userId = "ddee6c24";
            var user = _webShopContext.Useres.FirstOrDefault(u => u.Id== userId);

            var response = await _httpClient.DeleteAsync("user/delete/{userId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

            Assert.AreEqual("This user dosn't exist!", responseContent);
        }
    }
}

