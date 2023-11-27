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
        public async Task<IEnumerable<Product>> GetAllProductAsync()
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
        public async Task<Product> UpdateProduct(int productId, ProductDto product)
        {
            var productToUpdate = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (productToUpdate == null)
            {
                return null;
            }
            productToUpdate.ProductName = product.ProductName;
            productToUpdate.Description = product.Description;
            productToUpdate.Price = product.Price;
            productToUpdate.Stock = product.Stock;
            productToUpdate.Discount = product.Discount;
            productToUpdate.Category = product.Category;
            productToUpdate.SubCategory = product.SubCategory;
            productToUpdate.ImageBase64 = product.ImageBase64;
            _context.Products.Update(productToUpdate);
            await _context.SaveChangesAsync();
            return productToUpdate;
        }
        public async Task<ProductDto> GetProductById(int productId)
        {
            var product = await _context.Products
                .Where(p => p.ProductId == productId)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    Discount = p.Discount,
                    Category = p.Category,
                    SubCategory = p.SubCategory,
                    ImageBase64 = p.ImageBase64
                })
                .FirstOrDefaultAsync();
           
            return product;
        }
        public async Task<Product> DeleteProductById(int productId)
        {
            var deletedProduct = await _context.Products.FirstOrDefaultAsync(p=> p.ProductId == productId);
            if(deletedProduct == null)
            {
                return null;
            }
            _context.Products.Remove(deletedProduct);
            await _context.SaveChangesAsync();
            return deletedProduct;
        }
        public async Task<IEnumerable<Product>> GetProductsByCategory(int category)
        {
            var products = await _context.Products.Where(p => (int)p.Category == category).ToListAsync();
            return products;
        }
        public async Task<IEnumerable<Product>> GetProductsBySubCategory(int subCategory)
        {
            var products = await _context.Products.Where(p=>(int)p.SubCategory == subCategory).ToListAsync();
            return products;
        }
        public async Task<IEnumerable<Product>> GetProductsBySubAndMainCategory(int subCategory, int category)
        {
           var products = await _context.Products.Where(p => (int)p.SubCategory == subCategory && (int)p.Category == category).ToListAsync();

            return products;
        }
       
    }
}
