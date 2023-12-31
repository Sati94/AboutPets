﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WebShopAPI.Data;
using Microsoft.EntityFrameworkCore.InMemory;
using System.Net;
using WebShopAPI.Model;

namespace WebShopApiTest.IntegrationTest
{
    public class ProductControllerTest : WebApplicationFactory<Program>
    {
        private HttpClient _httpClient;
        private IAuthService _authService;
        private WebShopContext _webShopContext;
        [SetUp]
        public void Setup()
        {
            string connection = "Server=localhost,1433;Database=PetProject;User Id=sa;Password=SaraAttila1994;Encrypt=True;TrustServerCertificate=True;";
            Environment.SetEnvironmentVariable("CONNECTION_STRING", connection);
            var dbConnection = new DbContextOptionsBuilder<WebShopContext>()
             .UseSqlServer(connection)
                 .Options;
            _webShopContext = new WebShopContext(dbConnection);
            var option = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _httpClient = CreateClient();
            AuthRequest authRequest = new AuthRequest("admin@admin.com", "admin1234");
            string jsonString = JsonSerializer.Serialize(authRequest);
            StringContent jsonStringContent = new StringContent(jsonString);
            jsonStringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = _httpClient.PostAsync("/Login", jsonStringContent).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            var desContent = JsonSerializer.Deserialize<AuthResponse>(content, option);
            var token = desContent.Token;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            string userId = Guid.NewGuid().ToString();
            User newUser = new User { Id = userId , Email = "test@test.com", UserName = "Test1" };
            UserProfile newUserProfile = new UserProfile { FirstName = "Test", LastName = "Test", Address = "Test", PhoneNumber = "Test", Bonus = 0.1m, UserId = newUser.Id };
            newUser.Profile = newUserProfile;
            Product newProduct = new Product { ProductName = "Test", Description = "Test des", Price = 100, Stock = 100, Category = Category.Dog, SubCategory = SubCategory.WetFood, Discount = 0, ImageBase64 = "Test" };

            OrderItem newOrderItem = new OrderItem { Product = newProduct, Quantity = 10, Price = newProduct.Price * 10, UserId = newUser.Id };
            Order newOrder = new Order { OrderDate = DateTime.Now, OrderStatuses = OrderStatuses.Pending, UserId = newUser.Id, TotalPrice = newOrderItem.Price };

            newOrder.OrderItems.Add(newOrderItem);
            newUser.OrderItems.Add(newOrderItem);
            newUser.Orders.Add(newOrder);
            _webShopContext.Useres.Add(newUser);
            _webShopContext.UserProfiles.Add(newUserProfile);
            _webShopContext.Products.Add(newProduct);
            _webShopContext.Orders.Add(newOrder);
            _webShopContext.OrderItems.Add(newOrderItem);
            _webShopContext.SaveChanges();

        }
        [OneTimeTearDown]
        public void TearDown()
        {
            CleanUpDate();
            _httpClient.Dispose();
        }
        private void CleanUpDate()
        {
           

            var productsToDelete = _webShopContext.Products.Where(p => p.ProductName.Contains("Test")).ToList();
            var userDelete = _webShopContext.Useres.Where(u => u.UserName.Contains("Test")).ToList();
            var orderToDelete = _webShopContext.Orders.Where(o => o.User.UserName.Contains("Test")).ToList();
            var orderItemDelete = _webShopContext.OrderItems.Where(oi => oi.User.UserName == "Test").ToList();
            var userProfileToDelete = _webShopContext.UserProfiles.Where(up => up.User.UserName.Contains("Test")).ToList();

            _webShopContext.Products.RemoveRange(productsToDelete);
            _webShopContext.Useres.RemoveRange(userDelete);
            _webShopContext.Orders.RemoveRange(orderToDelete);
            _webShopContext.OrderItems.RemoveRange(orderItemDelete);
            _webShopContext.UserProfiles.RemoveRange(userProfileToDelete);
            _webShopContext.SaveChanges();
        }
        [Test]
        public async Task Return_AllProduct_Endpoint()
        {
            var response = await _httpClient.GetAsync("/product/available");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.IsNotEmpty(content);
        }
        [Test]
        public async Task Add_Product_Return_true()
        {
            ProductDto productDto = new ProductDto
            {
                ProductName = "Testek",
                Description = "finom kutya kaja",
                Price = 50,
                Stock = 50,
                Discount = 0,
                Category = Category.Dog,
                SubCategory = SubCategory.DryFood,
                ImageBase64 = "valami"
            };
            Product product = new Product
            {
                ProductName = productDto.ProductName,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                Discount = productDto.Discount,
                Category = productDto.Category,
                SubCategory = productDto.SubCategory,
                ImageBase64 = productDto.ImageBase64

            };

            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                productName = product.ProductName,
                description = product.Description,
                price = product.Price,
                stock = product.Stock,
                discount = product.Discount,
                category = (int)product.Category,
                subCategory = (int)product.SubCategory,
                imageBase64 = product.ImageBase64
            }), Encoding.UTF8, "application/json");

            var respose = await _httpClient.PostAsync("/create/product", content);
            respose.EnsureSuccessStatusCode();
            var responseContent = await respose.Content.ReadAsStringAsync();
            var createdProduct = await _webShopContext.Products.FirstOrDefaultAsync(p => p.ProductName == product.ProductName);
            Assert.NotNull(responseContent);
            Assert.AreEqual(productDto.ProductName, createdProduct.ProductName);
            Assert.AreEqual(productDto.Description, createdProduct.Description);
            Assert.AreEqual(productDto.Price, createdProduct.Price);
            Assert.AreEqual(productDto.Stock, createdProduct.Stock);
            Assert.AreEqual(productDto.Discount, createdProduct.Discount);
            Assert.AreEqual(productDto.Category, createdProduct.Category);
            Assert.AreEqual(productDto.SubCategory, createdProduct.SubCategory);
            Assert.AreEqual(productDto.ImageBase64, createdProduct.ImageBase64);
        }
        [Test]
        public async Task Delete_Product_NonExistingProduct_ReturnsNotFound()
        {
            int productId = 100;

            var response = await _httpClient.DeleteAsync($"/product/delete/{productId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Check if the response status code indicates a non-success status (e.g., 404 Not Found).
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

            // Check the response content for a specific message.
            Assert.AreEqual("This product doesn't exsist!", responseContent);
        }
        [Test]
        public async Task Find_Product_ById_RetrurnFalse()
        {
            var allProducts = _webShopContext.Products.OrderByDescending(p => p.ProductId).ToList();
            var lastProduct = allProducts.FirstOrDefault();
           
            int productId = lastProduct.ProductId;
            var product = _webShopContext.Products.FirstOrDefault(p => p.ProductId == productId);
            var response = await _httpClient.GetAsync($"/product/{productId}");
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(response);
            Assert.AreEqual(product.ProductId, productId);
          
        }
        
        [Test]
        public async Task Update_Product_Returns_OkResult()
        {
            var allProducts = _webShopContext.Products.OrderByDescending(p => p.ProductId).ToList();
            var lastProduct = allProducts.FirstOrDefault();


            int productId = lastProduct.ProductId;
           
            ProductDto updateProduct = new ProductDto
            {
                ProductName = "Testek",
                Description = "Nagy kutyáknak",
                Price = 50,
                Stock = 50,
                Discount = 0,
                Category = Category.Dog,
                SubCategory = SubCategory.DryFood,
                ImageBase64 = "valami"
            };
            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                productName = updateProduct.ProductName,
                description = updateProduct.Description,
                price = updateProduct.Price,
                stock = updateProduct.Stock,
                discount = updateProduct.Discount,
                category = (int)updateProduct.Category,
                subCategory = updateProduct.SubCategory,
                imageBase64 = updateProduct.ImageBase64,

            }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/product/update/{productId}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedProduct = JsonConvert.DeserializeObject<Product>(responseContent);
            Assert.AreEqual("Testek", updatedProduct.ProductName);
            Assert.AreEqual("Nagy kutyáknak", updatedProduct.Description);
            Assert.AreEqual(50, updatedProduct.Stock);
            Assert.AreEqual(0, updatedProduct.Discount);
            Assert.AreEqual(Category.Dog, updatedProduct.Category);
            Assert.AreEqual(SubCategory.DryFood, updatedProduct.SubCategory);
            Assert.AreEqual("valami", updatedProduct.ImageBase64);


        }
        [Test]
        public async Task Find_ProductByCategroy_Retrun_NotNull()
        {
            var category = Category.Dog;

            var response = await _httpClient.GetAsync($"/products/category/{category}");
            response.EnsureSuccessStatusCode();
            var respnseContent = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(respnseContent);
        }
        [Test]
        public async Task Find_ProductBySubCategory_Retrun_NotNull()
        {
            var subCategory = SubCategory.DryFood;

            var response = await _httpClient.GetAsync($"/products/category/subCategory/{subCategory}");
            response.EnsureSuccessStatusCode();
            var respnseContent = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(respnseContent);
        }

    }
}
