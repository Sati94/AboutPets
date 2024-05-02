using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private WebShopContext _webShopContext;
        private IProductService _productService;

        [SetUp]
        public void SetUp()
        {

            var options = new DbContextOptionsBuilder<WebShopContext>()
                         .UseInMemoryDatabase(databaseName: "TestDataBase")
                         .Options;

            _webShopContext = new WebShopContext(options);

            _productService = new ProductService(_webShopContext);
        }
        [TearDown]
        public void TearDown()
        {
            _webShopContext.Dispose();
        }

        [Test]
        public async Task GetAllProduct_ShouldReturnNotNull()
        {
            var product = new Product
            {
                ProductId = 1000,
                ProductName = "Test",
                Description = "Test des",
                Price = 100,
                Stock = 100,
                Discount = 0,
                Category = Category.Dog,
                SubCategory = SubCategory.WetFood,
                ImageBase64 = "Test"
            };
            var productList = new List<Product> { product };
            _webShopContext.Products.Add(product);
            await _webShopContext.SaveChangesAsync();


            // Act
            var result = await _productService.GetAllProductAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
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
            _webShopContext.Products.Add(newProduct);
            _webShopContext.SaveChanges();

            var result = await _productService.CreatePorductAsync(product);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductName, Is.EqualTo(product.ProductName));
        }
        [Test]
        public async Task UpdateProduct_ShouldReturnTrue()
        {
            var productDto = new ProductDto
            {
                ProductName = "UpdatedTestName",
                Description = "UpdatedDescription",
                Price = 2,
                Stock = 20,
                Discount = 0,
                CategoryId = 2,
                SubCategoryId = 5,
                ImageBase64 = "updated.jpg"
            };

            var product = new Product
            {
                ProductName = "OriginalName",
                Description = "OriginalDescription",
                Price = 1,
                Stock = 10,
                Discount = 0,
                Category = Category.Dog,
                SubCategory = SubCategory.WetFood,
                ImageBase64 = "original.jpg"
            };

            _webShopContext.Products.Add(product);
            _webShopContext.SaveChanges();

            // Act
            var result = await _productService.UpdateProduct(product.ProductId, productDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductName, Is.EqualTo(productDto.ProductName));
            Assert.That(result.Description, Is.EqualTo(productDto.Description));
            Assert.That(result.Price, Is.EqualTo(productDto.Price));
            Assert.That(result.Stock, Is.EqualTo(productDto.Stock));
            Assert.That(result.Discount, Is.EqualTo(productDto.Discount));
            Assert.That(result.ImageBase64, Is.EqualTo(productDto.ImageBase64));
        }
        [Test]
        public async Task GetProductById_ShouldReturnTrue()
        {
            var product = await _webShopContext.Products.FirstOrDefaultAsync();
            var productId = product.ProductId;

            var result = await _productService.GetProductById(productId);

            Assert.That(result, Is.Not.Null);
            Assert.That(productId, Is.EqualTo(result.ProductId));
        }
        [Test]
        public async Task DeleteProductById_ShouldReturnNull()
        {
          
            var product = await _webShopContext.Products.FirstOrDefaultAsync();
            var productId = product.ProductId;

            var act = await _productService.DeleteProductById(productId);
            var result = await _webShopContext.Products.FindAsync(productId);
            Assert.That(result,Is.Null);
        }
        [Test]
        public async Task GetProductsByCategory_ShouldReturnTrue()
        {
            var productList = await _webShopContext.Products.ToListAsync();

            var act = await _productService.GetProductsByCategory(1);

            var result = productList.Contains(act.FirstOrDefault());
            Assert.That(result, Is.True);

        }
        [Test]
        public async Task GetProductsBySubCategory_ShouldReturnTrue()
        {
            var productList = await _webShopContext.Products.ToListAsync();

            var act = await _productService.GetProductsBySubCategory(3);

            var result = productList.Contains(act.FirstOrDefault());
            Assert.That(result, Is.True);
        }
        
    }
}
