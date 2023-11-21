using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Model;
using WebShopAPI.Service.ProductServiceMap;

namespace WebShopAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("/create/product"), Authorize(Roles ="Admin")]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            var newProduct = _productService.CreatePorductAsync(product);
            return Ok(newProduct);
        }

    }
}
