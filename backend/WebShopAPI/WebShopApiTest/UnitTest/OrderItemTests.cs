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
        private WebShopContext _context;
        private IOrderItemService _orderItemService;

        [SetUp]
        public void SetUp()
        {
            var option = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestData")
                .Options;
            _context = new WebShopContext(option);
            SeedData.PopulateTestData(option);

            _orderItemService = new OrderItemService(_context);
        }
        [TearDown]
        public void TearDown()
        {

            CleanUpDate();

        }
        private void CleanUpDate()
        {
            if (_context != null)
            {
                var productsToDelete = _context.Products.Where(p => p.ProductName.Contains("Test")).ToList();
                var userDelete = _context.Useres.Where(u => u.UserName.Contains("Test")).ToList();
                var orderToDelete = _context.Orders.Where(o => o.User.UserName.Contains("Test")).ToList();
                var orderItemDelete = _context.OrderItems.Where(oi => oi.User.UserName == "Test").ToList();
                var userProfileToDelete = _context.UserProfiles.Where(up => up.User.UserName.Contains("Test")).ToList();

                _context.Products.RemoveRange(productsToDelete);
                _context.Useres.RemoveRange(userDelete);
                _context.Orders.RemoveRange(orderToDelete);
                _context.OrderItems.RemoveRange(orderItemDelete);
                _context.UserProfiles.RemoveRange(userProfileToDelete);

                _context.SaveChanges();
            }

        }
        [Test]
        public async Task AddOrderItemToUser_ReturnTrue()
        {
            var user = await _context.Useres.FirstOrDefaultAsync();
            var userId = user.Id;
            var product = await _context.Products.FirstOrDefaultAsync();
            var productId = product.ProductId;
            var quantity = 1;
            var order = await _context.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;

            var result = await _orderItemService.AddOrderItemToUser(userId, productId, quantity, orderId);

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);

        }
        [Test]
        public async Task DeleteOrderItemById_ShouldReturnIsNull()
        {
            var user = await _context.Useres.FirstOrDefaultAsync();
            var userId = user.Id;
            var orderItems = await _context.OrderItems.FirstOrDefaultAsync();
            var orderItemsId = orderItems.OrderId;

            var act = await _orderItemService.DeleteOrderItem(userId, orderItemsId);

            var result = await _context.OrderItems.FindAsync(orderItemsId);

            Assert.IsNull(result);
           
        }
        [Test]
        public async Task SetOrderItemQuantity_ShouldReturnTrue()
        {
            var user = await _context.Useres.FirstOrDefaultAsync();
            var userId = user.Id;
            var orderItems = await _context.OrderItems.FirstOrDefaultAsync();
            var orderItemsId = orderItems.OrderId;
            var newQuantity = 10;
            var act = await _orderItemService.SetOrderItemQuantity(userId, newQuantity, orderItemsId);
            var result = await _context.OrderItems.FindAsync(orderItemsId);

            Assert.AreEqual(result.Quantity, newQuantity);
        }


    }
}
