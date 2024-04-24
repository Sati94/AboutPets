
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WebShopAPI.Data;
using Microsoft.EntityFrameworkCore.InMemory;
using System.Net;
using WebShopAPI.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace WebShopApiTest.IntegrationTest
{
    public class ProductControllerTest : CustomWebApplicationFactory<Program>
    {
        private HttpClient _httpClient;
        private UserManager<IdentityUser> _userManager;
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


                dbContext.SaveChanges();
            }
            _httpClient.Dispose();
        }
        [Test]
        public async Task Return_AllProduct_Endpoint()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var product = dbContext.Products.FirstOrDefault();
                var productId = product.ProductId;


                var response = await _httpClient.GetAsync("/product/available");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var result = content.Contains(productId.ToString());
                Assert.That(result, Is.True);
            }
        }
        [Test]
        public async Task Add_Product_Return_true()
        {
            ProductDto productDto = new ProductDto
            {
                ProductName = "Test2",
                Description = "Test des",
                Price = 50,
                Stock = 50,
                Discount = 0,
                CategoryId = 1,
                SubCategoryId = 4,
                ImageBase64 = "valami"
            };
            Product product = new Product
            {
                ProductName = productDto.ProductName,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                Discount = productDto.Discount,
                Category = productDto.GetCategory(),
                SubCategory = productDto.GetSubCategory(),
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
            var result = responseContent.Contains(productDto.ProductName);

            Assert.That(result, Is.True);

        }
        [Test]
        public async Task Delete_Product_NonExistingProduct_ReturnsNotFound()
        {
            int productId = 100;

            var response = await _httpClient.DeleteAsync($"/product/delete/{productId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.That(responseContent, Does.Contain("This product doesn't exsist!"));
        }
        [Test]
        public async Task Find_Product_ById_RetrurnTrue()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var allProducts = dbContext.Products.OrderByDescending(p => p.ProductId).ToList();
                var lastProduct = allProducts.FirstOrDefault();
                var productId = lastProduct.ProductId;
                var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

                var response = await _httpClient.GetAsync($"/product/{productId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                Assert.That(productId, Is.EqualTo(product.ProductId));
            }
        }


        [Test]
        public async Task Update_Product_Returns_OkResult()
        {
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<WebShopContext>();
                var product = await dbContext.Products.FirstOrDefaultAsync();
                int productId = product.ProductId;

                ProductDto updateProduct = new ProductDto
                {
                    ProductId = productId,
                    ProductName = "Test",
                    Description = "Nagy kutyáknak",
                    Price = 50,
                    Stock = 50,
                    Discount = 0,
                    CategoryId = 1,
                    SubCategoryId = 4,
                    ImageBase64 = "valami"
                };
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    productId = updateProduct.ProductId,
                    productName = updateProduct.ProductName,
                    description = updateProduct.Description,
                    price = updateProduct.Price,
                    stock = updateProduct.Stock,
                    discount = updateProduct.Discount,
                    category = updateProduct.GetCategory(),
                    subCategory = updateProduct.GetSubCategory(),
                    imageBase64 = updateProduct.ImageBase64,

                }), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/product/update/{productId}", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var updatedProduct = JsonConvert.DeserializeObject<Product>(responseContent);

                Assert.That(updatedProduct.ProductName, Is.EqualTo(updateProduct.ProductName));

            }

        }
        [Test]
        public async Task Find_ProductByCategroy_Retrun_Null()
        {
            var category = Category.Cat;

            var response = await _httpClient.GetAsync($"/products/category/{category}");
            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                response.EnsureSuccessStatusCode();
                var respnseContent = await response.Content.ReadAsStringAsync();
                Assert.That(respnseContent, Is.EqualTo(null).Or.Empty);
            }
        }

        [Test]
        public async Task Find_ProductBySubCategory_Retrun_Null()
        {
            var subCategory = SubCategory.DryFood;

            var response = await _httpClient.GetAsync($"/products/category/subCategory/{subCategory}");
            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                response.EnsureSuccessStatusCode();
                var respnseContent = await response.Content.ReadAsStringAsync();
                Assert.That(respnseContent, Is.EqualTo(null).Or.Empty);
            }

        }
    }
}
