using WebShopAPI.Data;
using WebShopAPI.Model;
using WebShopAPI.Model.DTOS;
using WebShopAPI.Model.CategoryClasses;
using Microsoft.EntityFrameworkCore;

namespace WebShopAPI.Service.ProductServiceMap
{
    public class ProductService : IProductService
    {
        private readonly WebShopContext _context;
        public ProductService(WebShopContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAllProducAsync()
        {
            return await _context.Products.ToListAsync();
        }
        public async Task<Product> CreatePorductAsync(ProductDto product)
        {
             var newProduct  = new Product 
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
            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();
            return newProduct;
        }
    }
}
