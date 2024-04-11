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
        public async Task GetAllOrder_ShouldReturnIsNotNull()
        {
            var orders = await _context.Orders.ToListAsync();
            var order = orders.FirstOrDefault();
            bool result = orders.Contains(order);
            Assert.IsNotNull(orders);
            Assert.IsTrue(result);
        }

    }
}
/*Task<IEnumerable<Order>> GetAllOrderAsync();
Task<Order> GetOrderByIdAsync(int orderId);
Task<Order> GetOrderByUserId(string userId);
Task<Order> DeleteOrderById(int orderId);
Task<bool> UpdateOrderStatus(int orderId, OrderStatuses newStatus);
Task<bool> UpdateOrderTotlaPriceWithBonus(int orderId, string userId);*/