using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebShopAPI.Model;

namespace WebShopApiTest
{
    public class ProductControllerTest : WebApplicationFactory<Program>
    {
        private HttpClient _httpClient;
        private IAuthService _authService;
        [SetUp]
        public void Setup()
        {
            string connection = "Server=localhost,1433;Database=PetProject;User Id=sa;Password=SaraAttila1994;Encrypt=True;TrustServerCertificate=True;";
            Environment.SetEnvironmentVariable("CONNECTION_STRING", connection);
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
            _httpClient.Dispose();
        }
        [Test]
        public async Task Return_AllProduct_Endpoint()
        {
            var response = await _httpClient.GetAsync("/product/available");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.IsNotEmpty(content);
        }
    }
}
