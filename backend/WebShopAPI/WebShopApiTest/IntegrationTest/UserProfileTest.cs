﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Model.UserModels;

namespace WebShopApiTest.IntegrationTest
{
    public class UserProfileTest : WebApplicationFactory<Program>
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
        public async Task GetUserProfile_ByUserId_Return_True()
        {
            string userId = "ddee6c24-af15-4052-bb97-42ed4bf8a134";
            var userProfile = _webShopContext.UserProfiles.FirstOrDefault(up => up.UserId == userId);
            var response = await _httpClient.GetAsync($"user/profile/{userId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseContent);
            Assert.AreEqual(userProfile.UserId, userId);
        }
    }
}
