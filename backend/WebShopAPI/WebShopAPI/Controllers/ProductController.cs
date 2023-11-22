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
        [HttpPost("/create/product"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] ProductDto product)
        {
            var newProduct = await _productService.CreatePorductAsync(product);
            return Ok(newProduct);
        }
        [HttpGet("/product/avaiable"), Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<IEnumerable<Product>>> AllProductAsync()
        {
            var products = await _productService.GetAllProductAsync();
            return Ok(products);
        }
        [HttpPut("/product/update/{productId}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> UpdateProductAsync(int productId, ProductDto product)
        {
            var result = await _productService.UpdateProduct(productId, product);
            if(result == null)
            {
                return NotFound("This product doesn't exsist!");
            }
            return Ok(result);  
        }
        [HttpGet("/product/{productId}"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<ProductDto>> GetProductByIdAsync(int productId)
        {
            var product = await _productService.GetProductById(productId);
            if(product == null)
            {
                return NotFound("This product doesn't exsist!");
            }
            return Ok(product);
        }
        [HttpDelete("/product/delete/{productId}")]
        public async Task<ActionResult<Product>> DeleteProductByIdAsync(int productId)
        {
            var product = await _productService.DeleteProductById(productId);
            if( product == null)
            {
                return NotFound("This product doesn't exsist!");
            }
            return Ok(product);
        }

    }
}
