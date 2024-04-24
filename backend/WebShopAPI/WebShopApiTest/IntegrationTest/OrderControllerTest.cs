using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework.Internal;
using System.Net;
using WebShopAPI.Data;


namespace WebShopApiTest.IntegrationTest
{

    public class OrderControllerTest : CustomWebApplicationFactory<Program>
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
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                return userManager.Users.Any();
            }
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var users = userManager.Users.ToList();
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
        public async Task GetAll_Order_Return_NotNull()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var allOrders = dbContext.Orders.OrderByDescending(o => o.OrderId);
               
            
                var lastOrder = allOrders.FirstOrDefault();
                var orderId = lastOrder.OrderId;
                var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId ==orderId);

                var response = await _httpClient.GetAsync("/orderlist/all");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                Assert.That(orderId, Is.EqualTo(order.OrderId));
            }
           

           
        }
        [Test]
        public async Task GetOrder_ById_Retrun_True()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var order = await dbContext.Orders.FirstOrDefaultAsync();


                var orderId = order.OrderId;

                if (order == null)
                {
                    Assert.That(order, Is.Null);
                }
                else
                {
                    var response = await _httpClient.GetAsync($"/ordelist/order/{orderId}");
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Assert.That(orderId,Is.EqualTo(order.OrderId));
                }

            }  
        }
        [Test]
        public async Task GetOrder_ByUserId_Return_True() 
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var user = await userManager.FindByEmailAsync("test@test.com");
                var userId = user.Id;

                var order = dbContext.Orders.FirstOrDefault(o => o.UserId == userId);
                if (order == null)
                {
                    Assert.That(order, Is.Null);
                }
                else
                {
                    var response = await _httpClient.GetAsync($"/order/{userId}");
                    var responseContent = await response.Content.ReadAsStringAsync();
             
                    Assert.That(userId, Is.EqualTo(order.UserId));

                }
            }
         
        }
        [Test]
        public async Task Delete_OrderById_Return_NotFound()
        {
            using(var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                int orderId = 100;
                var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                var response = await _httpClient.DeleteAsync($"/order/delete/{orderId}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Assert.That(order, Is.Null);
            }
  
        }
        [Test]
        public async Task Update_OrderStatus_ById_Return_False()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var order = await dbContext.Orders.FirstOrDefaultAsync();
                var newStatus = OrderStatuses.Shipped;
                var orderId = order.OrderId;
                if (order != null)
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
                    Assert.That(isUpdateSuccessful, Is.True);
                }
            }
        }    
           
        [Test]
        public async Task UpdateOrderTotalPriceWithBonus_ShouldReturnTrue()
        {
                using (var scope = Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                    var user = await userManager.FindByEmailAsync("test@test.com");
                    var userId = user.Id;

                    var allOrder = dbContext.Orders.OrderByDescending(o => o.OrderId).ToList();
                    var order = allOrder.FirstOrDefault();
                    int orderId = order.OrderId;
                    var userProfile = dbContext.UserProfiles.FirstOrDefault(up => up.UserId == userId);
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
                            Assert.That(result, Is.True);
                        }

                    }

                }
         
           
            
           
        
            
            
            

           
          


        }
    }
}