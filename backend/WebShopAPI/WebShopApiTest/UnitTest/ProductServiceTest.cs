using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebShopAPI.Data;
using WebShopAPI.Model;
using WebShopApiTest.IntegrationTest;

namespace WebShopApiTest.UnitTest
{
    /*public class ProductServiceTest
    {
        private WebShopContext _webShopContext;
        private IProductService _productService;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<WebShopContext>()
                .UseInMemoryDatabase(databaseName: "TestDataBase")
                .Options;
            _webShopContext = new WebShopContext(options);
            SeedData.PopulateTestData(options);
            

            _productService = new ProductService(_webShopContext);
        }
        [TearDown]
        public void TearDown()
        {

            CleanUpDate();

        }
        private void CleanUpDate()
        {
            if (_webShopContext != null)
            {
                var productsToDelete = _webShopContext.Products.Where(p => p.ProductName.Contains("Test")).ToList();
                var productsToDelete2 = _webShopContext.Products.Where(p => p.ProductName.Contains("Test2")).ToList();
                var userDelete = _webShopContext.Users.Where(u => u.UserName.Contains("Test")).ToList();
                var orderToDelete = _webShopContext.Orders.Where(o => o.User.UserName.Contains("Test")).ToList();
                var orderItemDelete = _webShopContext.OrderItems;
                var userProfileToDelete = _webShopContext.UserProfiles.Where(up => up.User.UserName.Contains("Test")).ToList();

                _webShopContext.Products.RemoveRange(productsToDelete);
                _webShopContext.Products.RemoveRange(productsToDelete2);
                _webShopContext.Users.RemoveRange(userDelete);
                _webShopContext.Orders.RemoveRange(orderToDelete);

                _webShopContext.UserProfiles.RemoveRange(userProfileToDelete);
                _webShopContext.SaveChanges();
            }

        }

        [Test]
        public async Task GetAllProduct_ShouldReturnNotNull()
        {
            var productList = await _webShopContext.Products.ToListAsync();

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
                CategoryId = 2,
                SubCategoryId = 4,
                ImageBase64 = "jpg"
                
            };
            var newProduct = new Product
            {
            
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Discount = product.Discount,
                Category = product.GetCategory(),
                SubCategory = product.GetSubCategory(),
                ImageBase64 = product.ImageBase64
              
            };


            var result = await _productService.CreatePorductAsync(product);               

            Assert.NotNull(result);
            Assert.AreEqual(result.ProductName, product.ProductName);
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
                CategoryId = 1,
                SubCategoryId = 4,
                ImageBase64 = "jpg"
            };
            var product = await _webShopContext.Products.FirstOrDefaultAsync();
            var productId = product.ProductId;
            var result = await _productService.UpdateProduct(productId, productDto);

            Assert.NotNull(result);
            Assert.AreEqual(result.ProductName, productDto.ProductName);
        }
        [Test]
        public async Task GetProductById_ShouldReturnTrue()
        {
            var product = await _webShopContext.Products.FirstOrDefaultAsync();
            var productId = product.ProductId;

            var result = await _productService.GetProductById(productId);

            Assert.NotNull(result);
            Assert.AreEqual(productId, result.ProductId);
        }
        [Test]
        public async Task DeleteProductById_ShouldReturnNull()
        {
          
            var product = await _webShopContext.Products.FirstOrDefaultAsync();
            var productId = product.ProductId;

            var act = await _productService.DeleteProductById(productId);
            var result = await _webShopContext.Products.FindAsync(productId);
            Assert.IsNull(result);
        }
        [Test]
        public async Task GetProductsByCategory_ShouldReturnTrue()
        {
            var productList = await _webShopContext.Products.ToListAsync();

            var act = await _productService.GetProductsByCategory(1);

            var result = productList.Contains(act.FirstOrDefault());
            Assert.True(result);

        }
        [Test]
        public async Task GetProductsBySubCategory_ShouldReturnTrue()
        {
            var productList = await _webShopContext.Products.ToListAsync();

            var act = await _productService.GetProductsBySubCategory(3);

            var result = productList.Contains(act.FirstOrDefault());
            Assert.True(result);
        }
        
    }*/
}
