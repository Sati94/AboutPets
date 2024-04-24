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
        

        public  async Task PopulateTestDataAsync(WebShopContext dbContext, UserManager<IdentityUser> userManager)
        {
            if (dbContext != null &&  userManager != null)
            {
                var newUser = new IdentityUser
                {
                    UserName = "Test",
                    Email = "test@test.com"
                };
                

                var userCreated = await userManager.CreateAsync(newUser, "Test123");

                if (userCreated.Succeeded)
                {

                    UserProfile newUserProfile = new UserProfile
                    {
                        FirstName = "Test",
                        LastName = "Test",
                        Address = "Test",
                        PhoneNumber = "Test",
                        Bonus = 1,
                        UserId = newUser.Id,

                    };


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

                    };


                    newOrder.OrderItems.Add(newOrderItem);


                    dbContext.UserProfiles.Add(newUserProfile);
                    dbContext.Products.Add(newProduct);
                    dbContext.Orders.Add(newOrder);
                    dbContext.OrderItems.Add(newOrderItem);

                    dbContext.SaveChanges();
                }
                else 
                {
                    foreach (var error in userCreated.Errors)
                    {
                        Console.WriteLine($"Hiba: {error.Description}");
                    }
                   
                }
               
         
            }
            
        }
    }
}





           