using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Model;
using WebShopApiTest.IntegrationTest;

namespace WebShopApiTest.UnitTest
{
    public class ProductServiceTest
    {
        private WebShopContext _context;
        private IProductService _productService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;
            _context = new WebShopContext(options);
            SeedData.PopulateTestData(options);
            var productServiceMock = new Mock<IProductService>();

            _productService = new ProductService(_context);
        }
        [Test]
        public async Task GetAllProduct_ShouldReturnNotNull()
        {
            var productList = await _context.Products.ToListAsync();

            var result = await _productService.GetAllProductAsync();
            Assert.NotNull(result);
            Assert.That(result, Is.EqualTo(productList));

        }
        [Test]
        public async Task CreateProduct_ShouldReturnTrue()
        {
            var product = new ProductDto
            {
                ProductName = "Test2",
                Description = "Valami",
                Price = 1,
                Stock = 10,
                Discount =0,
                Category = Category.Cat,
                SubCategory = SubCategory.WetFood,
                ImageBase64 = "jpg"
            };
            var newProduct = new Product
            {
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Discount = product.Discount,
                Category = product.Category,
                SubCategory = product.SubCategory,
                ImageBase64 = product.ImageBase64
            };
            var result = await _productService.CreatePorductAsync(product);


            Assert.NotNull(result);
            Assert.AreEqual(result.ProductName, newProduct.ProductName);
        }
        [Test]
        public async Task UpdateProduct_ShouldReturnTrue()
        {
            var productDto = new ProductDto
            {
                ProductName = "Test2",
                Description = "Valami",
                Price = 1,
                Stock = 10,
                Discount = 0,
                Category = Category.Cat,
                SubCategory = SubCategory.WetFood,
                ImageBase64 = "jpg"
            };
            var product = await _context.Products.FirstOrDefaultAsync();
            var productId = product.ProductId;
            var result = await _productService.UpdateProduct(productId, productDto);

            Assert.NotNull(result);
            Assert.AreEqual(result.ProductName, productDto.ProductName);
        }
       

    }
}
