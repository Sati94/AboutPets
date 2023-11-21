using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopAPI.Model;
using WebShopAPI.Model.DTOS;
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
        public async Task<ActionResult<Product>> CreateProduct([FromBody] ProductDto product)
        {
            var newProduct = await _productService.CreatePorductAsync(product);
            return Ok(newProduct);
        }

    }
}
