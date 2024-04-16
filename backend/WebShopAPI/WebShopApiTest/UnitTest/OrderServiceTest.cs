using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopApiTest.IntegrationTest;

namespace WebShopApiTest.UnitTest
{
    public class OrderServiceTest
    {
        private WebShopContext _context;
        private IOrderService _orderService;

        [SetUp]
        public void SetUp() 
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;
            _context = new WebShopContext(options);
            SeedData.PopulateTestData(options);

            _orderService = new OrderService(_context);
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
                var userDelete = _context.Users.Where(u => u.UserName.Contains("Test")).ToList();
                var orderToDelete = _context.Orders.Where(o => o.User.UserName.Contains("Test")).ToList();
                var orderItemDelete = _context.OrderItems.Where(oi => oi.User.UserName == "Test").ToList();
                var userProfileToDelete = _context.UserProfiles.Where(up => up.User.UserName.Contains("Test")).ToList();

                _context.Products.RemoveRange(productsToDelete);
                _context.Users.RemoveRange(userDelete);
                _context.Orders.RemoveRange(orderToDelete);
                _context.OrderItems.RemoveRange(orderItemDelete);
                _context.UserProfiles.RemoveRange(userProfileToDelete);

                _context.SaveChanges();
            }

        }
        [Test]
        public async Task GetAllOrder_ShouldReturnIsNotNull()
        {
            var result = await _orderService.GetAllOrderAsync();
           
            Assert.IsNotNull(result);
        }
        [Test]
        public async Task GetOrderById_ShouldReturnTrue()
        {
            var order = await _context.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;
            var result = await _orderService.GetOrderByIdAsync(orderId);

            Assert.IsNotNull(result);
            Assert.AreEqual(orderId, result.OrderId);
        }
        [Test]
        public async Task GetOrderByUserId_ShioldReturnTrue()
        {
            var user = await _context.Users.FirstOrDefaultAsync();
            string userId = user.Id;
            var result = await _orderService.GetOrderByUserId(userId);
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
        }
        [Test]
        public async Task DeleteOrderById_ShouldReturnIsNull()
        {
            var order = await _context.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;

            var act = await _orderService.DeleteOrderById(orderId);

            var result = await _context.Orders.FindAsync(orderId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateOrderStatus_ShouldReturnIsTrue()
        {
            var order = await _context.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;
            OrderStatuses newStatus = OrderStatuses.Cancelled;

            var result = await _orderService.UpdateOrderStatus(orderId, newStatus);

            Assert.IsTrue(result);

        }
        [Test]
        public async Task UpdateOrderTotalPriceWithBonus_ShouldReturnTrue()
        {
            var order = await _context.Orders.FirstOrDefaultAsync();
            var orderId = order.OrderId;
            var userId = order.UserId;

            var result = await _orderService.UpdateOrderTotlaPriceWithBonus(orderId, userId);

            Assert.IsTrue(result);
        }
    }
}
/*



Task<bool> UpdateOrderTotlaPriceWithBonus(int orderId, string userId);*/