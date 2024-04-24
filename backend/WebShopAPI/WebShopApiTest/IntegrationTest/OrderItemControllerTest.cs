using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebShopAPI.Data;
using WebShopAPI.Model;
using WebShopAPI.Model.OrderModel;
using WebShopAPI.Model.UserModels;

namespace WebShopApiTest.IntegrationTest
{
    /*public class OrderItemControllerTest : CustomWebApplicationFactory<Program>
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
        public async Task AddOrderItemToUserAsync_ReturnsOkWithOrderItem()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var user = await _userManager.FindByEmailAsync("test@test.com");
                var userId = user.Id;

                var product = await dbContext.Products.FirstOrDefaultAsync();
                var productId = product.ProductId;


                var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.UserId == userId);
                var orderItems = order.OrderItems.FirstOrDefault();

                var orderId = order.OrderId;
                var quantity = 1;

                var orderItem = new OrderItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price * quantity,
                    OrderId = orderId,
                    Product = product,
                    Order = order

                };
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    productId = orderItem.ProductId,
                    quantity = orderItem.Quantity,
                    price = orderItem.Price,
                    orderId = orderItem.OrderId,
                    product = orderItem.Product,
                    order = orderItem.Order


                }), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/add?userId={userId}&productId={productId}&quantity={quantity}&orderid={orderId}", content);
               
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();


            }
                

        }
        [Test]
        public async Task Delete_OrderItem_NonExistingProduct_ReturnsNotFound()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var user = await _userManager.FindByEmailAsync("test@test.com");

                var userId = user.Id;

                var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.UserId == userId);

                var orderId = order.OrderId;

                var orderItem = order.OrderItems.FirstOrDefault();
                var orderItemId = orderItem.OrderItemId;

                var response = await _httpClient.DeleteAsync($"/orderitem/remove?orderId={orderId}&orderItemId={orderItemId}&userId={userId}");

                Assert.That(order.OrderItems.Contains(orderItem), Is.False);


                if (order.OrderItems.Count == 0)
                {
                    Assert.That(dbContext.OrderItems.Find(orderItem.OrderItemId), Is.Null);
                }


                Assert.That(dbContext.OrderItems.Find(orderItemId), Is.Null);



            }
        }
    }*/
    
}
