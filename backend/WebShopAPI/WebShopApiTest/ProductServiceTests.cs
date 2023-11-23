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
    }
}