using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Data;
using WebShopApiTest.IntegrationTest;

namespace WebShopApiTest.UnitTest
{
    public class OrderServiceTest
    {
        private WebShopContext _webShopContext;
        private IOrderService _orderService;
        private Mock<UserManager<IdentityUser>> _mockUserManager;

        [SetUp]
        public void SetUp() 
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var options = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;
            _webShopContext = new WebShopContext(options);
       

            _orderService = new OrderService(_webShopContext);
        }
        [TearDown]
        public void TearDown()
        {

            _webShopContext.Dispose();

        }
        
        [Test]
        public async Task GetAllOrder_ShouldReturnIsNotNull()
        {
            var user = new IdentityUser
            {
                Id = "123456asd",
                UserName = "test2",
                Email = "test2@test.com"
            };
            _mockUserManager.Setup(u => u.FindByIdAsync("123456asd")).ReturnsAsync(user);

            var userProfile = new UserProfile
            {
                FirstName = "Test",
                LastName = "Test",
                UserId = user.Id,
                Address = "Test",
                PhoneNumber = "Test",
                Bonus = 0
            };

            var product = new Product
            {
                ProductId = 100,
                Stock = 20,
                Price = 10,
                ProductName = "TestProduct",
                Description = "TestDescription",
                ImageBase64 = "TestImageBase64"
            };

            var orderItem = new OrderItem
            {
                OrderItemId = 100,
                OrderId = 1,
                ProductId = 1,
                Price = 10,
                Quantity = 5
            };
           
            var order = new Order
            {
                OrderId = 100,
                OrderDate = DateTime.Now,
                OrderStatuses = OrderStatuses.Pending,
                UserId = user.Id
            };
            order.OrderItems.Add(orderItem);
            _webShopContext.UserProfiles.Add(userProfile);
            _webShopContext.Products.Add(product);
            _webShopContext.OrderItems.Add(orderItem);
            _webShopContext.Orders.Add(order);
            await _webShopContext.SaveChangesAsync();

            var result = await _orderService.GetAllOrderAsync();
           
            Assert.That(result, Is.Not.Null);
            Assert.That(100, Is.EqualTo(order.OrderId));
        }
        [Test]
        public async Task GetOrderById_ShouldReturnTrue()
        {
            var order = await _webShopContext.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;
            var result = await _orderService.GetOrderByIdAsync(orderId);

            Assert.That(result, Is.Not.Null);
            Assert.That(orderId, Is.EqualTo(order.OrderId));
        }
        [Test]
        public async Task GetOrderByUserId_ShioldReturnTrue()
        {
            var userId = "123456asd";
           
            var result = await _orderService.GetOrderByUserId(userId);
            Assert.That(result, Is.Not.Null);
            Assert.That(userId, Is.EqualTo(result.UserId));
        }
       

        [Test]
        public async Task UpdateOrderStatus_ShouldReturnIsTrue()
        {
            var order = await _webShopContext.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;
            OrderStatuses newStatus = OrderStatuses.Cancelled;

            var result = await _orderService.UpdateOrderStatus(orderId, newStatus);

            Assert.That(result, Is.True);

        }
       
       [Test]
       public async Task UpdateOrderTotalPriceWithBonus_ShouldReturnTrue()
       {
           var order = await _webShopContext.Orders.FirstOrDefaultAsync();
           var orderId = order.OrderId;
           var userId = order.UserId;

           var result = await _orderService.UpdateOrderTotlaPriceWithBonus(orderId, userId);

           Assert.That(result, Is.True);
       }
       [Test]
       public async Task DeleteOrderById_ShouldReturnIsNull()
       {
           var order = await _webShopContext.Orders.FirstOrDefaultAsync();
           var orderId = order.OrderId;

           var act = await _orderService.DeleteOrderById(orderId);

           var result = await _webShopContext.Orders.FindAsync(orderId);

           Assert.That(result, Is.Null);

       }
    }
}




