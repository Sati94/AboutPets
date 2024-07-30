using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Model.TodoItem;
using WebShopAPI.Model.TodoItem.TodoItemStatus;
using WebShopAPI.Service.NotificatonServiceMap;

namespace WebShopApiTest.IntegrationTest
{
    public class NotificationIntegrationTest : CustomWebApplicationFactory<Program>
    {
        private HttpClient _httpClient;
        private UserManager<IdentityUser> _userManager;
        private INotificationService _notificationService;
        private AuthService _authService;
        private async Task InitializeTestDataAsync()
        {

            var scope = Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var dbContext = scopedServices.GetRequiredService<WebShopContext>();
            _userManager = scopedServices.GetRequiredService<UserManager<IdentityUser>>();
            var seedData = new SeedData();
            await seedData.PopulateTestDataAsync(dbContext, _userManager);
            await WaitForDatabase();
        }
        [OneTimeSetUp]
        public async Task Setup()
        {
            await InitializeTestDataAsync();
            _httpClient = CreateClient();
            _authService = new AuthService(_httpClient);
            var token = _authService.AuthenticateAndGetToken("admin@admin.com", "admin1234");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");


        }
        private async Task WaitForDatabase()
        {
            int maxRetryCount = 5;
            int retryDelayMilliseconds = 1000;

            for (int i = 0; i < maxRetryCount; i++)
            {
                if (IsDatabaseReady())
                {
                    return;
                }

                await Task.Delay(retryDelayMilliseconds);
            }
        }
        private bool IsDatabaseReady()
        {
            using (var scope = Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                return userManager.Users.Any();
            }
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var users = userManager.Users.ToList();
                foreach (var user in users)
                {
                    await userManager.DeleteAsync(user);
                }
                dbContext.Products.RemoveRange(dbContext.Products);
                dbContext.Orders.RemoveRange(dbContext.Orders);
                dbContext.OrderItems.RemoveRange(dbContext.OrderItems);
                dbContext.UserProfiles.RemoveRange(dbContext.UserProfiles);
                dbContext.TodoItems.RemoveRange(dbContext.TodoItems);


                dbContext.SaveChanges();
            }
            _httpClient.Dispose();
        }
        [Test]
        public async Task GetAllTodo_ShouldReturnTodoItems()
        {
            // Act
            var response = await _httpClient.GetAsync("/notifications/all");

            // Assert
            response.EnsureSuccessStatusCode();
            var todoItems = await response.Content.ReadFromJsonAsync<IEnumerable<TodoItem>>();

            Assert.That(todoItems, Is.Not.Null);
            Assert.That(todoItems.Count(), Is.GreaterThan(0));
        }
        [Test]
        public async Task AddNotification_ShouldReturnTodoItem_WhenModelIsValid()
        {
            // Arrange
            var todoModel = new TodoModel
            {
                Sender = "System",
                Title = "New Todo",
                Description = "This is a new todo item"
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync("/notifications/todo/addNotification", todoModel);

            // Assert
            response.EnsureSuccessStatusCode();
            var todoItem = await response.Content.ReadFromJsonAsync<TodoItem>();

            Assert.That(todoItem, Is.Not.Null);
            Assert.That(todoItem.Title, Is.EqualTo(todoModel.Title));
            Assert.That(todoItem.Description, Is.EqualTo(todoModel.Description));
           
        }
        [Test]
        public async Task DeleteTodoItem_ShouldReturnTrue_WhenTodoItemExists()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                int todoId = 10;
                var todoItem = new TodoItem
                  {
                     Id = todoId,
                     Title = "Test Todo",
                     Description = "Test Description",
                     Sender = "Test Sender",
                     CreatedDate = DateTime.UtcNow,
                     Status = TodoStatus.New
                  };

                dbContext.TodoItems.Add(todoItem);
                await dbContext.SaveChangesAsync();

                var response = await _httpClient.DeleteAsync($"/notifications/deleteTodoItem/{todoId}");

                // Assert - Check that the response is successful
                response.EnsureSuccessStatusCode();

                // Verify that the TodoItem was deleted from the database
                var deletedTodoItem = await dbContext.TodoItems.FindAsync(todoId);
                if (deletedTodoItem != null)
                {
                    // Optionally log or print the details of the TodoItem for debugging purposes
                    Console.WriteLine($"Deleted TodoItem details: Id = {deletedTodoItem.Id}, Title = {deletedTodoItem.Title}");
                }

                // Assert that the TodoItem is not present in the database
                Assert.That(deletedTodoItem, Is.Null, "TodoItem should be deleted and not found in the database.");
            }
        
        }
    }
}
