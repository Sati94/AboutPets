using WebShopAPI.Data;
using WebShopAPI.Model;
using WebShopAPI.Model.CategoryClasses;

namespace WebShopAPI.Service.ProductServiceMap
{
    public class ProductService : IProductService
    {
        private readonly WebShopContext _context;
        public ProductService(WebShopContext context)
        {
            _context = context;
        }
        public async Task<Product> CreatePorductAsync(Product product)
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
