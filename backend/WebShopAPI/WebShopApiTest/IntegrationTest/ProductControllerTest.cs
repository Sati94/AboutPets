
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WebShopAPI.Data;
using Microsoft.EntityFrameworkCore.InMemory;
using System.Net;
using WebShopAPI.Model;

namespace WebShopApiTest.IntegrationTest
{
    public class ProductControllerTest : CustomWebApplicationFactory<Program>
    {
        private HttpClient _httpClient;
        private WebShopContext _webShopContext;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                 .UseInMemoryDatabase(databaseName: "InMemoryWebShopContext")
                 .Options;

            _webShopContext = new WebShopContext(options);
            SeedData.PopulateTestData(options);

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


        }
        [TearDown]
        public void TearDown()
        {
            CleanUpDate();
            _httpClient.Dispose();
        }
        private void CleanUpDate()
        {
           

            var productsToDelete = _webShopContext.Products.Where(p => p.ProductName.Contains("Test")).ToList();
            var productsToDelete2 = _webShopContext.Products.Where(p => p.ProductName.Contains("Test2")).ToList();
            var userDelete = _webShopContext.Useres.Where(u => u.UserName.Contains("Test")).ToList();
            var orderToDelete = _webShopContext.Orders.Where(o => o.User.UserName.Contains("Test")).ToList();
            var orderItemDelete = _webShopContext.OrderItems.Where(oi => oi.User.UserName == "Test").ToList();
            var userProfileToDelete = _webShopContext.UserProfiles.Where(up => up.User.UserName.Contains("Test")).ToList();

            _webShopContext.Products.RemoveRange(productsToDelete);
            _webShopContext.Products.RemoveRange(productsToDelete2);
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
                ProductName = "Test2",
                Description = "Test des",
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
            Assert.NotNull(responseContent);
            
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
            var product = await _webShopContext.Products.FirstOrDefaultAsync();
            int productId = product.ProductId;
           
            ProductDto updateProduct = new ProductDto
            {
                ProductId = productId,
                ProductName = "Test",
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
                productId = updateProduct.ProductId,
                productName = updateProduct.ProductName,
                description = updateProduct.Description,
                price = updateProduct.Price,
                stock = updateProduct.Stock,
                discount = updateProduct.Discount,
                category = (int)updateProduct.Category,
                subCategory = (int)updateProduct.SubCategory,
                imageBase64 = updateProduct.ImageBase64,

            }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/product/update/{productId}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedProduct = JsonConvert.DeserializeObject<Product>(responseContent);
            Assert.IsNotNull(updatedProduct);


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
