using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebShopAPI.Model;
using WebShopAPI.Model.OrderModel;
using WebShopAPI.Model.UserModels;

namespace WebShopApiTest.IntegrationTest
{
    /*public class OrderItemControllerTest : CustomWebApplicationFactory<Program>
    {
        private HttpClient _httpClient;
        private WebShopContext _webShopContext;
        private UserManager<IdentityUser> _userManager;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                 .UseInMemoryDatabase(databaseName: "InMemoryWebShopContext")
                 .Options;

            _webShopContext = new WebShopContext(options);
            SeedData.PopulateTestData(options);
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_webShopContext), null, null, null, null, null, null, null, null);


            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var order = _webShopContext.Orders.FirstOrDefault();
            var user = _webShopContext.Users.FirstOrDefault();

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


            var productsToDelete = _webShopContext.Products.Where(p => p.ProductName.Contains("Test")).ToList();
            var productsToDelete2 = _webShopContext.Products.Where(p => p.ProductName.Contains("Test2")).ToList();
            var userDelete = _webShopContext.Users.Where(u => u.UserName.Contains("Test")).ToList();
            var orderToDelete = _webShopContext.Orders.Where(o => o.User.UserName.Contains("Test")).ToList();
            var orderItemDelete = _webShopContext.OrderItems;
            var userProfileToDelete = _webShopContext.UserProfiles.Where(up => up.User.UserName.Contains("Test")).ToList();

            _webShopContext.Products.RemoveRange(productsToDelete);
            _webShopContext.Products.RemoveRange(productsToDelete2);
            _webShopContext.Users.RemoveRange(userDelete);
            _webShopContext.Orders.RemoveRange(orderToDelete);

            _webShopContext.UserProfiles.RemoveRange(userProfileToDelete);
            _webShopContext.SaveChanges();
        }
        [Test]
        public async Task AddOrderItemToUserAsync_ReturnsOkWithOrderItem()
        {
            var user = await _webShopContext.Users.FirstOrDefaultAsync();
            var userId = user.Id; 
   
            var product = await _webShopContext.Products.FirstOrDefaultAsync();
            var productId = product.ProductId;

         
            var order = user.Orders.FirstOrDefault();
            var orderItems = order.OrderItems.FirstOrDefault();
;           var orderId = order.OrderId;
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
                order = orderItem.Order,
               

            }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"/add?userId={userId}&productId={productId}&quantity={quantity}&orderid={orderId}", content);
            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var returnedOrderItem = JsonSerializer.Deserialize<OrderItem>(responseContent, options);
            
            var createdOrderItem = await _webShopContext.OrderItems.FirstOrDefaultAsync(oi => oi.OrderId == orderItem.OrderId);

            Assert.IsNotNull(returnedOrderItem);
            Assert.IsNotNull(createdOrderItem);

        }
        /*[Test]
        public async Task Delete_OrderItem_NonExistingProduct_ReturnsNotFound()
        {
            var userId = "TestUser";

            // Add test data to the in-memory database
            var user = new User { Id = userId, UserName = "TestUser", Email = "test@test.com" };

            var order = new Order { OrderDate = DateTime.MinValue, OrderStatuses = OrderStatuses.Cancelled, UserId = userId };
            var orderid = order.OrderId;

            var product = new Product { Description = "Valami", ProductName = "Test", Price = 10, Category = Category.Cat, SubCategory = SubCategory.WetFood, Discount = 0, Stock = 10, ImageBase64 = "valami" };

            // Add the product to the context and save changes
            _webShopContext.Products.Add(product);
            await _webShopContext.SaveChangesAsync();

            // Now create the order item using the product ID
            var orderItem = new OrderItem { Quantity = 1, Price = 0, OrderId = orderid, ProductId = product.ProductId };

            
            order.OrderItems.Add(orderItem);

            _webShopContext.Users.Add(user);
            _webShopContext.Orders.Add(order);
            _webShopContext.OrderItems.Add(orderItem);
            await _webShopContext.SaveChangesAsync();

            var orderItemId = orderItem.OrderItemId;

            var orderItemService = new OrderItemService(_webShopContext, _userManager);

            // Act
            var result = await orderItemService.DeleteOrderItem(userId, orderItemId);

            Assert.IsFalse(user.OrderItems.Contains(orderItem));
            Assert.IsFalse(order.OrderItems.Contains(orderItem));

            // If the order had only this item, ensure that the order is removed
            if (order.OrderItems.Count == 0)
            {
                Assert.IsNull(_webShopContext.Orders.Find(order.OrderId));
            }

            // Ensure that the OrderItem is removed from the context
            Assert.IsNull(_webShopContext.OrderItems.Find(orderItemId));

            // Ensure that the Product still exists in the context
            var productInContext = _webShopContext.Products.Find(product.ProductId);
            Assert.IsNotNull(productInContext);
        

            
        }
    }*/
    
}
