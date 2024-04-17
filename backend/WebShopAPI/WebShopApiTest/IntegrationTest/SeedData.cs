using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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
        
        
        public static void PopulateTestData(DbContextOptions<WebShopContext> dbContextOptions )
        {
            

            using (var dbContext = new WebShopContext(dbContextOptions))
            {
                

               
                User newUser = new User
                {
                    Id = "1234",
                    Email = "test@test.com",
                    UserName = "Test"
                };
                


                UserProfile newUserProfile = new UserProfile 
                {
                    FirstName = "Test",
                    LastName = "Test",
                    Address = "Test",
                    PhoneNumber = "Test",
                    Bonus = 1,
                    UserId = newUser.Id,
                    User = newUser 
                };
                newUser.Profile = newUserProfile;

                Product newProduct = new Product 
                {
                    ProductId = 100,
                    ProductName = "Test",
                    Description = "Test des",
                    Price = 100,
                    Stock = 100,
                    Discount = 0,
                    Category = Category.Dog,
                    SubCategory = SubCategory.WetFood,
                    ImageBase64 = "Test" 
                };

                OrderItem newOrderItem = new OrderItem
                { 
                    Product = newProduct,
                    Quantity = 10,
                    Price = newProduct.Price * 10 
                };

                Order newOrder = new Order 
                { 
                    OrderDate = DateTime.Now,
                    OrderStatuses = OrderStatuses.Pending,
                    UserId = newUser.Id,
                    TotalPrice = newOrderItem.Price,
                    User = newUser 
                };


                newOrder.OrderItems.Add(newOrderItem);
                newUser.Orders.Add(newOrder);

                dbContext.Users.Add(newUser);
                dbContext.UserProfiles.Add(newUserProfile);
                dbContext.Products.Add(newProduct);
                dbContext.Orders.Add(newOrder);
                dbContext.OrderItems.Add(newOrderItem);

                dbContext.SaveChanges();

            }
        }
    }
}
