using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShopApiTest.IntegrationTest
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public  string AuthenticateAndGetToken(string email, string password) 
        {
            var authRequest = new AuthRequest(email, password);
            var jsonString = JsonSerializer.Serialize(authRequest);
            var jsonStringContent = new StringContent(jsonString);
            jsonStringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = _httpClient.PostAsync("/Login", jsonStringContent).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var desContent = JsonSerializer.Deserialize<AuthResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return  desContent.Token;
        }
    }
}
