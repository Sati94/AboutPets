using Azure;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Model;
using WebShopAPI.Model.OrderModel;

namespace WebShopApiTest.IntegrationTest
{
    public class OrderControllerTest : WebApplicationFactory<Program>
    {
        private  HttpClient _httpClient;
        private  WebShopContext _webShopContext;
        private IAuthService _authService;
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
        public async Task GetAll_Order_Return_NotNull()
        {
            var response = await _httpClient.GetAsync("/orderlist/all");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
            Assert.IsNotEmpty(content);
        }
        [Test]
        public async Task GetOrder_ById_Retrun_True()
        {
            int orderId = 11;
            var order = _webShopContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if(order == null)
            {
                Assert.IsNull(order);
            }
            else
            {
                var response = await _httpClient.GetAsync($"/ordelist/order/{orderId}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Assert.NotNull(responseContent);
                Assert.AreEqual(orderId, order.OrderId);
            }
        }
        [Test]
        public async Task GetOrder_ByUserId_Return_True() 
        {
            string userId = "ddee6c24-af15-4052-bb97-42ed4bf8a134";
            var order = _webShopContext.Orders.FirstOrDefault(o => o.UserId == userId);
            if (order == null)
            {
                Assert.IsNull(order);
            }
            else
            {
                var response = await _httpClient.GetAsync($"/order/{userId}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Assert.NotNull(responseContent);
                Assert.AreEqual(userId, order.UserId);
            }
        }
        [Test]
        public async Task Delete_OrderById_Return_NotFound()
        {
            int orderId = 100;
            
            var response = await _httpClient.DeleteAsync($"/order/delete/{orderId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
  
        }
        [Test]
        public async Task Update_OrderStatus_ById_Return_True() 
        {
            int orderId = 11;
            var newStatus = OrderStatuses.Shipped;
            var order =  _webShopContext.Orders.FirstOrDefault(o=> o.OrderId == orderId);
            if(order != null)
            {
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    orderId = order.OrderId,
                    orderdate = order.OrderDate,
                    totalprice = order.TotalPrice,
                    orderStatuses = newStatus,
                    userId = order.UserId

                }), Encoding.UTF8, "application/json"); ;
                var response = await _httpClient.PutAsync($"/order/update/{orderId}", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var isUpdateSuccessful = JsonConvert.DeserializeObject<bool>(responseContent);
                Assert.IsTrue(isUpdateSuccessful);
            }
        }
    }
}
