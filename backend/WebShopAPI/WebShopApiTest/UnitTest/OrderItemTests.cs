using FluentAssertions.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Data;
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

        [SetUp]
        public void SetUp()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            var options = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;

            _webShopContext = new WebShopContext(options);


            _orderItemService = new OrderItemService(_webShopContext, _mockUserManager.Object);
        }
        [TearDown]
        public void TearDown()
        {

            _webShopContext.Dispose();

        }
       
        [Test]
        public async Task AddOrderItemToUser_ReturnTrue()
        {
            var user = new IdentityUser
            {
                Id = "123456asd",
                UserName = "test2",
                Email = "test2@test.com"
            };
            _mockUserManager.Setup(u => u.FindByIdAsync("123456asd")).ReturnsAsync(user);

         

            var product = new Product { 
                ProductId = 1, 
                Stock = 20, 
                Price = 10,
                ProductName = "TestProduct",
                Description = "TestDescription",
                ImageBase64 = "TestImageBase64"
            };
            _webShopContext.Products.Add(product);
            await _webShopContext.SaveChangesAsync();

            var userId = user.Id;
            var productId = product.ProductId;
            var quantity = 5;
            var orderId = 0;
            var result = await _orderItemService.AddOrderItemToUser(userId, productId, quantity, orderId);
            Assert.That(result, Is.Not.Null);
            Assert.That(productId, Is.EqualTo(result.ProductId));
            Assert.That(quantity, Is.EqualTo(result.Quantity));

        }
        
        [Test]
        public async Task SetOrderItemQuantity_ShouldReturnTrue()
        {
            var order = new Order { OrderId = 20 , UserId = "123456asd" };
           
            var orderItem = new OrderItem { OrderItemId = 20, OrderId = order.OrderId, Quantity = 5 , Price = 1000, ProductId = 1};
            order.OrderItems.Add(orderItem);
            _webShopContext.Orders.Add(order);

            _webShopContext.OrderItems.Add(orderItem);
            await _webShopContext.SaveChangesAsync();

            var newQuantity = 10;

            // Act
            var result = await _orderItemService.SetOrderItemQuantity(order.OrderId, orderItem.OrderItemId, newQuantity);

            // Assert
            Assert.That(result,Is.Not.Null);
            Assert.That(result.Quantity, Is.EqualTo(newQuantity));

          
            
        }
        [Test]
        public async Task DeleteOrderItemById_ShouldReturnIsNull()
        {
            var userId = "123456asd";
            var orderItems = await _webShopContext.OrderItems.FirstOrDefaultAsync();
            var orderItemId = orderItems.OrderId;
            var order = await _webShopContext.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;
            var act = await _orderItemService.DeleteOrderItem(orderId, orderItemId, userId);

            var result = await _webShopContext.OrderItems.FindAsync(orderItemId);

            Assert.That(act, Is.Null);
            Assert.That(result.OrderItemId, Is.EqualTo(orderItemId));

        }


    }
}
