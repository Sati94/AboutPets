using WebShopAPI.Model;
using WebShopAPI.Model.CategoryClasses;
using WebShopAPI.Model.DTOS;

namespace WebShopAPI.Service.ProductServiceMap
{
    public interface IProductService
    {
        Task<IEnumerable<Product?>> GetAllProductAsync(); 
        Task<Product> CreatePorductAsync(ProductDto product);
        Task<Product> UpdateProduct(int productId, ProductDto product);
        Task<ProductDto> GetProductById(int productId);
        Task<Product> DeleteProductById(int productId);
        Task<List<Product>> GetProductsByCategory(int category);
    }
}
