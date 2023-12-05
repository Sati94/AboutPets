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
    public class OrderItemControllerTest : WebApplicationFactory<Program>
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
            var userDelete = _webShopContext.Useres.Where(u => u.Id.Contains("Test")).ToList();
            var orderToDelete = _webShopContext.Orders.Where(o => o.UserId.Contains("Test")).ToList();
            var orderItemDelete = _webShopContext.OrderItems.Where(oi => oi.UserId == "Test").ToList();

            _webShopContext.Products.RemoveRange(productsToDelete);
            _webShopContext.Useres.RemoveRange(userDelete);
            _webShopContext.Orders.RemoveRange(orderToDelete);
            _webShopContext.OrderItems.RemoveRange(orderItemDelete);
            _webShopContext.SaveChanges();
        }
        [Test]
        public async Task AddOrderItemToUserAsync_ReturnsOkWithOrderItem()
        {
            // Arrange
            var userId = "Test";
            var productId = 1;
            var quantity = 10;
            var orderid = 1;
            var user = await _webShopContext.Useres.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                // Létrehozunk egy új felhasználót, vagy más módon inicializáljuk
                user = new User { Id = userId, UserName = "TestUser", Email = "test@test.com" };
                _webShopContext.Useres.Add(user);
                await _webShopContext.SaveChangesAsync();
            }
            var product = await _webShopContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                // Létrehozunk egy új felhasználót, vagy más módon inicializáljuk

                product = new Product { Description = "Valami", ProductName = "Test", Price = 10, Category = Category.Cat, SubCategory = SubCategory.WetFood, Discount = 0, Stock = 10, ImageBase64 = "valami" };

                _webShopContext.Products.Add(product);
                await _webShopContext.SaveChangesAsync();
                productId = product.ProductId;
            }
            var order = await _webShopContext.Orders.FirstOrDefaultAsync(o => o.OrderId == orderid);
            if (order == null)
            {
                // Létrehozunk egy új felhasználót, vagy más módon inicializáljuk
                order = new Order { OrderDate = DateTime.MinValue, OrderStatuses = OrderStatuses.Cancelled, UserId = userId };
                _webShopContext.Orders.Add(order);
                await _webShopContext.SaveChangesAsync();
            }

            // Előre létrehozott OrderItem
            var orderItem = new OrderItem
            {
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price * quantity,
                OrderId = orderid,
                UserId = userId,
                Product = product,
                Order = order,
                User = user,

            };

            var response = await _httpClient.PostAsync($"/add?userId={userId}&productId={productId}&quantity={quantity}&orderid={orderid}", null);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };


            var returnedOrderItem = JsonSerializer.Deserialize<OrderItem>(responseContent, options);
            Console.WriteLine(responseContent);
            Console.WriteLine(JsonSerializer.Serialize(returnedOrderItem, options));
            // Itt várható, hogy a válasz tartalmazza az OrderItem adatait
            var createdOrderItem = await _webShopContext.OrderItems.FirstOrDefaultAsync(oi => oi.UserId == orderItem.UserId);


            Assert.IsNotNull(returnedOrderItem);
            Assert.IsNotNull(createdOrderItem);



        }
        [Test]
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
            var orderItem = new OrderItem { UserId = userId, Quantity = 1, Price = 0, OrderId = orderid, ProductId = product.ProductId };

            user.OrderItems.Add(orderItem);
            order.OrderItems.Add(orderItem);

            _webShopContext.Useres.Add(user);
            _webShopContext.Orders.Add(order);
            _webShopContext.OrderItems.Add(orderItem);
            await _webShopContext.SaveChangesAsync();

            var orderItemId = orderItem.OrderItemId;

            var orderItemService = new OrderItemService(_webShopContext);

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
    }
    
}
