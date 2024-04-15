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
            

            _productService = new ProductService(_context);
        }
        [TearDown]
        public void TearDown()
        {

            CleanUpDate();

        }
        private void CleanUpDate()
        {
            if (_context != null)
            {
                var productsToDelete = _context.Products.Where(p => p.ProductName.Contains("Test")).ToList();
                var userDelete = _context.Useres.Where(u => u.UserName.Contains("Test")).ToList();
                var orderToDelete = _context.Orders.Where(o => o.User.UserName.Contains("Test")).ToList();
                var orderItemDelete = _context.OrderItems.Where(oi => oi.User.UserName == "Test").ToList();
                var userProfileToDelete = _context.UserProfiles.Where(up => up.User.UserName.Contains("Test")).ToList();

                _context.Products.RemoveRange(productsToDelete);
                _context.Useres.RemoveRange(userDelete);
                _context.Orders.RemoveRange(orderToDelete);
                _context.OrderItems.RemoveRange(orderItemDelete);
                _context.UserProfiles.RemoveRange(userProfileToDelete);

                _context.SaveChanges();
            }

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
        [Test]
        public async Task GetProductById_ShouldReturnTrue()
        {
            var product = await _context.Products.FirstOrDefaultAsync();
            var productId = product.ProductId;

            var result = await _productService.GetProductById(productId);

            Assert.NotNull(result);
            Assert.AreEqual(productId, result.ProductId);
        }
        [Test]
        public async Task DeleteProductById_ShouldReturnNull()
        {
          
            var product = await _context.Products.FirstOrDefaultAsync();
            var productId = product.ProductId;

            var act = await _productService.DeleteProductById(productId);
            var result = await _context.Products.FindAsync(productId);
            Assert.IsNull(result);
        }
        [Test]
        public async Task GetProductsByCategory_ShouldReturnTrue()
        {
            var productList = await _context.Products.ToListAsync();

            var act = await _productService.GetProductsByCategory(1);

            var result = productList.Contains(act.FirstOrDefault());
            Assert.True(result);

        }
        [Test]
        public async Task GetProductsBySubCategory_ShouldReturnTrue()
        {
            var productList = await _context.Products.ToListAsync();

            var act = await _productService.GetProductsBySubCategory(3);

            var result = productList.Contains(act.FirstOrDefault());
            Assert.True(result);
        }
        
    }
}
