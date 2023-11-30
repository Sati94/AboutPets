
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WebShopAPI.Data;
using Microsoft.EntityFrameworkCore.InMemory;

namespace WebShopApiTest
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
            var options = new JsonSerializerOptions
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
            var desContent = JsonSerializer.Deserialize<AuthResponse>(content, options);
            var token = desContent.Token;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            
        }
        [OneTimeTearDown] 
        public void TearDown() 
        {
            CleanUpDate();
            _httpClient.Dispose();
        }
        private void CleanUpDate()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                 .UseInMemoryDatabase(databaseName: "TestDatabase")
                 .Options;

            var productsToDelete = _webShopContext.Products.Where(p => p.ProductName.Contains("Test")).ToList();

            _webShopContext.Products.RemoveRange(productsToDelete);
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
                ProductName = "Kutya Kaja",
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
    }
}
