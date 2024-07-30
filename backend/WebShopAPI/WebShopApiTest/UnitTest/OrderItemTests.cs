using FluentAssertions.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Data;
using WebShopAPI.Model;
using WebShopAPI.Model.TodoItem;
using WebShopAPI.Service.NotificatonServiceMap;
using WebShopAPI.Model.TodoItem.TodoItemStatus;
using WebShopAPI.Service.OrderItemServiceMap;
using WebShopApiTest.IntegrationTest;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebShopApiTest.UnitTest
{
    public class OrderItemTests
    {
        private WebShopContext _webShopContext;
        private IOrderItemService _orderItemService;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private INotificationService _notificationService;

        [SetUp]
        public void SetUp()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var options = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;

            _webShopContext = new WebShopContext(options);

            _webShopContext.Database.EnsureCreated();

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock
                .Setup(n => n.CheckProductStock(It.IsAny<int>()))
                .ReturnsAsync((int productId) => {
                    var product = _webShopContext.Products.Find(productId);
                    if (product != null && product.Stock <= 10)
                    {
                        var todoItem = new TodoItem
                        {
                            // Initialize TodoItem properties accordingly
                            Id = 1, // or a suitable value
                            Sender = "System",
                            Title = "Stock Alert",
                            Description = $"Product {product.ProductName} stock is low. Now it is {product.Stock}. You need to talk to the warehouse!",
                            CreatedDate = DateTime.Now,
                            Status = TodoStatus.New,
                        };
                        return todoItem;
                    }
                    return null;
                });

            _notificationService = notificationServiceMock.Object;
            _orderItemService = new OrderItemService(_webShopContext, _mockUserManager.Object,_notificationService);

            var user = new IdentityUser
            {
                Id = "123456asd",
                UserName = "test2",
                Email = "test2@test.com"
            };
            _mockUserManager.Setup(u => u.FindByIdAsync("123456asd")).ReturnsAsync(user);

            var product = new Product
            {
                ProductId = 1,
                Stock = 20,
                Price = 10,
                ProductName = "TestProduct",
                Description = "TestDescription",
                ImageBase64 = "TestImageBase64"
            };
            _webShopContext.Products.Add(product);

            var order = new Order
            {
                OrderId = 1,
                OrderDate = DateTime.Now,
                OrderStatuses = OrderStatuses.Pending,
                Country = "TestCountry",
                City = "TestCity",
                StreetAddress = "TestAddress",
                UserId = user.Id
            };
            _webShopContext.Orders.Add(order);

            _webShopContext.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _webShopContext.Database.EnsureDeleted();
            _webShopContext.Dispose();

        }
       
        [Test]
        public async Task AddOrderItemToUser_ReturnTrue()
        {
            var userId = "123456asd";
            var productId = 1;
            var quantity = 5;
            var orderId = 1;

            var result = await _orderItemService.AddOrderItemToUser(userId, productId, quantity, orderId);

         
            Assert.That(productId, Is.EqualTo(result.ProductId));
            Assert.That(quantity, Is.EqualTo(result.Quantity));
        }
        
        [Test]
        public async Task SetOrderItemQuantity_ShouldReturnTrue()
        {
            var userId = "123456asd";
            var orderItemId = 2;
            var orderId = 2;
            var productId = 1;

 
            var order = new Order
            {
                OrderId = orderId,
                OrderDate = DateTime.Now,
                OrderStatuses = OrderStatuses.Pending,
                Country = "TestCountry",
                City = "TestCity",
                StreetAddress = "TestAddress",
                UserId = userId
            };
            _webShopContext.Orders.Add(order);

            var orderItem = new OrderItem
            {
                OrderItemId = orderItemId,
                OrderId = 2,
                Quantity = 5,
                Price = 100,
                ProductId = productId,
            };
            _webShopContext.OrderItems.Add(orderItem);
            await _webShopContext.SaveChangesAsync();
            var newQuantity = 10;

            var result = await _orderItemService.SetOrderItemQuantity(order.OrderId, orderItem.OrderItemId, newQuantity);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Quantity, Is.EqualTo(newQuantity));


        }
        [Test]
        public async Task DeleteOrderItemById_ShouldReturnIsNull()
        {
            var userId = "123456asd";
            var orderId = 1;
            var orderItemId = 1;

            var act = await _orderItemService.DeleteOrderItem(orderId, orderItemId, userId);

            var result = await _webShopContext.OrderItems.FindAsync(orderItemId);

            Assert.That(act, Is.Null);
            Assert.That(result, Is.Null);

        }


    }
}
