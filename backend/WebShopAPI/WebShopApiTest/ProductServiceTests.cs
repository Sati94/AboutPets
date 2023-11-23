using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebShopAPI.Data;
using WebShopAPI.Model.DTOS;
using WebShopAPI.Model;
using WebShopAPI.Service.ProductServiceMap;
using WebShopAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Model.CategoryClasses;

namespace WebShopApiTest
{
   
    [TestFixture]
    public class ProductServiceTest
    {
        private Mock<IProductService> _productServiceMock;
        private ProductController _productController;

        [SetUp]
        public void Setup()
        {
            _productServiceMock = new Mock<IProductService>();
            _productController = new ProductController(_productServiceMock.Object);
        }

        [Test]
        public async Task GetAllProducts_ShouldReturnAllProducts()
        {
            var products = new List<Product>
            {
                new Product {ProductName = "Product1", ProductId = 1},
                new Product {ProductName = "Product2", ProductId = 2}
            };
            _productServiceMock.Setup(service => service.GetAllProductAsync()).ReturnsAsync(products);

            var result = await _productController.AllProductAsync();
            Assert.IsInstanceOf<ActionResult<IEnumerable<Product>>>(result);

            var actionResult = result as ActionResult<IEnumerable<Product>>;
            Assert.NotNull(actionResult);

            var okObjectResult = actionResult.Result as OkObjectResult;
            Assert.NotNull(okObjectResult);

            var actualProducts = okObjectResult.Value as IEnumerable<Product>;
            Assert.NotNull(actualProducts);

            var productCount = actualProducts.Count();
            Assert.AreEqual(products.Count, productCount);
        }
        [Test]
        public async Task UpdateProductAsync_ShouldUpdateProduct()
        {
            int productId = 1; 
            var productDto = new ProductDto
            {
                ProductName = "UpdatedProductName",
                Description = "UpdatedDescription",
                Price = 300,
                Stock = 20,
                Discount = 5,
                Category = Category.Dog, 
                SubCategory = SubCategory.Games, 
                ImageBase64 = "updatedImageBase64"
                
            };

            // Konfiguráljuk a mock ProductService-t
            _productServiceMock.Setup(service => service.UpdateProduct(productId, productDto))
                              .ReturnsAsync(new Product
                              {
                                  ProductId = productId,
                                  ProductName = productDto.ProductName,
                                  Description = productDto.Description,
                                  Price = productDto.Price,
                                  Stock = productDto.Stock,
                                  Discount = productDto.Discount,
                                  Category = productDto.Category,
                                  SubCategory = productDto.SubCategory,
                                  ImageBase64 = productDto.ImageBase64
                                  // Add other properties as needed
                              });

         
            var result = await _productController.UpdateProductAsync(productId, productDto);
            Assert.NotNull(result); 
        }
    }
    
}