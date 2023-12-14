using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Data;

namespace WebShopApiTest.IntegrationTest
{
    public class SeedData
    {
        public static void PopulateTestData(DbContextOptions<WebShopContext> dbContextOptions)
        {

            using (var dbContext = new WebShopContext(dbContextOptions))
            {
                string userId = Guid.NewGuid().ToString();

                User newUser = new User { Id = userId, Email = "test@test.com", UserName = "Test2" };

                UserProfile newUserProfile = new UserProfile { FirstName = "Test", LastName = "Test", Address = "Test", PhoneNumber = "Test", Bonus = 0.1m, UserId = newUser.Id };
                newUser.Profile = newUserProfile;

                Product newProduct = new Product { ProductName = "Test", Description = "Test des", Price = 100, Stock = 100, Category = Category.Dog, SubCategory = SubCategory.WetFood, Discount = 0, ImageBase64 = "Test" };

                OrderItem newOrderItem = new OrderItem { Product = newProduct, Quantity = 10, Price = newProduct.Price * 10, UserId = newUser.Id };

                Order newOrder = new Order { OrderDate = DateTime.Now, OrderStatuses = OrderStatuses.Pending, UserId = newUser.Id, TotalPrice = newOrderItem.Price };

                newOrder.OrderItems.Add(newOrderItem);
                newUser.OrderItems.Add(newOrderItem);
                newUser.Orders.Add(newOrder);

                dbContext.Useres.Add(newUser);
                dbContext.UserProfiles.Add(newUserProfile);
                dbContext.Products.Add(newProduct);
                dbContext.Orders.Add(newOrder);
                dbContext.OrderItems.Add(newOrderItem);

                dbContext.SaveChanges();

            }
        }
    }
}
