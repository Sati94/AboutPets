using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework.Internal;
using System.Net;


namespace WebShopApiTest.IntegrationTest
{

    public class OrderControllerTest : CustomWebApplicationFactory<Program>
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

        [TearDown]
        public void TearDown()
        {
            
                CleanUpDate();
                _httpClient.Dispose();
                
            

        }
        private void CleanUpDate()
        {
            if(_webShopContext != null)
            {
                var productsToDelete = _webShopContext.Products.Where(p => p.ProductName.Contains("Test")).ToList();
                var userDelete = _webShopContext.Users.Where(u => u.UserName.Contains("Test")).ToList();
                var orderToDelete = _webShopContext.Orders.Where(o => o.User.UserName.Contains("Test")).ToList();
                var orderItemDelete = _webShopContext.OrderItems.Where(oi => oi.User.UserName == "Test").ToList();
                var userProfileToDelete = _webShopContext.UserProfiles.Where(up => up.User.UserName.Contains("Test")).ToList();

                _webShopContext.Products.RemoveRange(productsToDelete);
                _webShopContext.Users.RemoveRange(userDelete);
                _webShopContext.Orders.RemoveRange(orderToDelete);
                _webShopContext.OrderItems.RemoveRange(orderItemDelete);
                _webShopContext.UserProfiles.RemoveRange(userProfileToDelete);
                _webShopContext.SaveChanges();
            }
            Console.WriteLine("nincs adatbázis");
           
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

            var order = await _webShopContext.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;
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
            var user = await _webShopContext.Users.FirstOrDefaultAsync();
            var userId = user.Id;
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
        public async Task Update_OrderStatus_ById_Return_False() 
        {
           
            var newStatus = OrderStatuses.Shipped;
            var order = await _webShopContext.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;
            if(order != null)
            {
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    orderid = orderId,
                    orderdate = order.OrderDate,
                    totalprice = order.TotalPrice,
                    orderStatuses = newStatus,
                    userId = order.UserId

                }), Encoding.UTF8, "application/json"); ;
                var response = await _httpClient.PutAsync($"/order/update/{orderId}", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var isUpdateSuccessful = JsonConvert.DeserializeObject<bool>(responseContent);
                Assert.IsFalse(isUpdateSuccessful);
            }
        }
        [Test]
        public async Task UpdateOrderTotalPriceWithBonus_ShouldReturnTrue()
        {
          
            var user = _webShopContext.Users.FirstOrDefault(u => u.UserName == "Test");
            string userId = user.Id;
            
            var allOrder = _webShopContext.Orders.OrderByDescending(o => o.OrderId).ToList();
            var order = allOrder.FirstOrDefault();
        
            int orderId = order.OrderId;
            
            var userProfile = _webShopContext.UserProfiles.FirstOrDefault(up => up.UserId == userId);

            if (user != null && order != null && userProfile != null)
            {
                var bonus = userProfile.Bonus;

                if (bonus > 0)
                {
                    var newTotalPrice = order.TotalPrice - (order.TotalPrice * bonus);
                    var content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        orderId = order.OrderId,
                        orderdate = order.OrderDate,
                        totalprice = newTotalPrice,
                        orderStatuses = order.OrderStatuses,
                        userId = order.UserId
                    }), Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync($"/order/{orderId}/apply-cupon/{userId}", content);
                    response.EnsureSuccessStatusCode();


                    var responseContent = await response.Content.ReadAsStringAsync();
    
                    var result = JsonConvert.DeserializeObject<bool>(responseContent);
                    Assert.IsFalse(result);
                }
               
            }
          


        }
    }
}
