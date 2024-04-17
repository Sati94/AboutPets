using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Data;
using WebShopApiTest.IntegrationTest;

namespace WebShopApiTest.UnitTest
{
    public class OrderItemTests
    {
        private WebShopContext _webShopContext;
        private IOrderItemService _orderItemService;
        private UserManager<IdentityUser> _userManager;

        [SetUp]
        public void SetUp()
        {
            var option = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestData")
                .Options;
            _webShopContext = new WebShopContext(option);
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_webShopContext), null, null, null, null, null, null, null, null);
            SeedData.PopulateTestData(option);

            _orderItemService = new OrderItemService(_webShopContext, _userManager);
        }
        [TearDown]
        public void TearDown()
        {

            CleanUpDate();

        }
        private void CleanUpDate()
        {
            if (_webShopContext != null)
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

        }
        [Test]
        public async Task AddOrderItemToUser_ReturnTrue()
        {
            var user = await _webShopContext.Users.FirstOrDefaultAsync();
            var userId = user.Id;
            var product = await _webShopContext.Products.FirstOrDefaultAsync();
            var productId = product.ProductId;
            var quantity = 1;
            var order = await _webShopContext.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;

            var result = await _orderItemService.AddOrderItemToUser(userId, productId, quantity, orderId);

            Assert.IsNotNull(result);
            Assert.AreEqual(orderId, result.OrderId);

        }
        [Test]
        public async Task DeleteOrderItemById_ShouldReturnIsNull()
        {
            var user = await _webShopContext.Users.FirstOrDefaultAsync();
            var userId = user.Id;
            var orderItems = await _webShopContext.OrderItems.FirstOrDefaultAsync();
            var orderItemId = orderItems.OrderId;
            var order = await _webShopContext.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;
            var act = await _orderItemService.DeleteOrderItem(orderId , orderItemId, userId);

            var result = await _webShopContext.OrderItems.FindAsync(orderItemId);

            Assert.IsNull(result);
           
        }
        [Test]
        public async Task SetOrderItemQuantity_ShouldReturnTrue()
        {
            var order = await _webShopContext.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;
            var orderItems = await _webShopContext.OrderItems.FirstOrDefaultAsync();
            var orderItemsId = orderItems.OrderId;
            var newQuantity = 10;
            var act = await _orderItemService.SetOrderItemQuantity(orderId, newQuantity, orderItemsId);
            var result = await _webShopContext.OrderItems.FindAsync(orderItemsId);

            Assert.AreEqual(result.Quantity, newQuantity);
        }


    }
}
