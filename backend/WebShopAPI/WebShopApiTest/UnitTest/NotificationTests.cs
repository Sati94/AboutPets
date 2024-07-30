using Microsoft.AspNetCore.Identity;
using WebShopAPI.Model.TodoItem;
using WebShopAPI.Service.NotificatonServiceMap;
using WebShopAPI.Model.TodoItem.TodoItemStatus;
using WebShopAPI.Model.OrderModel.DeliveryType;


namespace WebShopApiTest.UnitTest
{
    public class NotificationServiceTest
    {
        private WebShopContext _webShopContext;
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
            _notificationService = new NotificationService(_webShopContext, _mockUserManager.Object);

            _webShopContext.Database.EnsureCreated();
        }
        [TearDown]
        public void TearDown()
        {
            _webShopContext.Database.EnsureDeleted();
            _webShopContext.Dispose();

        }
        [Test]
        public async Task ListAllTodo_ShouldReturnTodoItems()
        {
            var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = 1, Title = "Test 1", Description="Test", Sender = "Test", CreatedDate = DateTime.Now, Status = TodoStatus.New },
            new TodoItem { Id= 2, Title = "Test 2", Description="Test", Sender = "Test", CreatedDate = DateTime.Now, Status = TodoStatus.New }
        };

           _webShopContext.TodoItems.AddRange(todoItems);
            await _webShopContext.SaveChangesAsync();

            var result = await _notificationService.ListAllTodo();

            Assert.That(result, Is.Not.Null);
            Assert.That(2, Is.EqualTo(result.Count()));
            Assert.That("Test 1",Is.EqualTo( result.First().Title));
        }
        [Test]
        public async Task CheckUserSpending_ShouldReturnTodoItem_WhenOrderTotalExceedsThreshold()
        {
            // Arrange
            var user = new IdentityUser { Id = "user123", UserName = "TestUser", Email = "testuser@example.com" };
            _mockUserManager.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(user);

            var order = new Order
            {
                OrderId = 1,
                UserId = "user123",
                TotalPrice = 150,
                Country = "Test",
                City = "Test",
                StreetAddress = "Test",
                OrderDate = DateTime.Now,
                OrderStatuses = OrderStatuses.Processing,
                DeliveryType = DeliveryTypes.Post
            };

            _webShopContext.Orders.Add(order);
            await _webShopContext.SaveChangesAsync();

            
            var result = await _notificationService.CheckUserSpending("user123", 1);

            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Bonus Alert"));
            Assert.That(result.Description, Contains.Substring("spent over $100"));
        }
        [Test]
        public async Task CheckProductStock_ShouldReturnTodoItem_WhenStockIsLow()
        {
            
            var product = new Product
            {
                ProductId = 1,
                ProductName = "TestProduct",
                Stock = 5,
                Price = 10,
                SubCategory = SubCategory.WetFood,
                Category = Category.Cat,
                Description = "Test",
                Discount = 0,
                ImageBase64 = "string",
            };

            _webShopContext.Products.Add(product);
            await _webShopContext.SaveChangesAsync();

            
            var result = await _notificationService.CheckProductStock(1);

            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Stock Alert"));
            Assert.That(result.Description, Contains.Substring("Product TestProduct stock is low"));
        }
        [Test]
        public async Task NewOrderReceived_ShouldReturnTodoItem_WhenOrderExists()
        {
            // Arrange
            var user = new IdentityUser { Id = "user123", UserName = "TestUser", Email = "testuser@example.com" };
            _mockUserManager.Setup(u => u.FindByIdAsync("user123")).ReturnsAsync(user);

            var order = new Order
            {
                OrderId = 1,
                UserId = "user123",
                Country = "test",
                City = "test", 
                StreetAddress = "test",
                OrderDate = DateTime.Now,
                OrderStatuses = OrderStatuses.Processing,
                DeliveryType = DeliveryTypes.DPD,
                TotalPrice = 10
            };

            _webShopContext.Orders.Add(order);
            await _webShopContext.SaveChangesAsync();

            // Act
            var result = await _notificationService.NewOrderReceived(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("New Order"));
            Assert.That(result.Description, Contains.Substring("New order received from user123"));
        }
        [Test]
        public async Task DeleteTodoItem_ShouldReturnTrue_WhenTodoItemExists()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 100,
                Title = "TestTodo",
                Description = "TestDescription",
                Sender = "TestSender",
                CreatedDate = DateTime.Now,
                Status = TodoStatus.New
            };

            _webShopContext.TodoItems.Add(todoItem);
            await _webShopContext.SaveChangesAsync();

            
            var result = await _notificationService.DeleteTodoItem(100);

        
            Assert.That(result, Is.True);

        }
        [Test]
        public async Task UpdatetodoItemStatus_ShouldReturnTrue_WhenTodoItemExists()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 10,
                Title = "TestTodo",
                Description = "TestDescription",
                Sender = "TestSender",
                CreatedDate = DateTime.Now,
                Status = TodoStatus.New
            };

            _webShopContext.TodoItems.Add(todoItem);
            await _webShopContext.SaveChangesAsync();

            // Act
            var result = await _notificationService.UpdatetodoItemStatus(10, TodoStatus.InProcess);

            // Assert
            Assert.That(result, Is.True);
            var updatedItem = await _webShopContext.TodoItems.FindAsync(10);
            Assert.That(updatedItem.Status, Is.EqualTo(TodoStatus.InProcess));
        }
    }
}
