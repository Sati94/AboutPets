using WebShopAPI.Model;
using WebShopAPI.Model.DTOS;

namespace WebShopAPI.Service.ProductServiceMap
{
    public interface IProductService
    {
        Task<IEnumerable<Product?>> GetAllProducAsync(); 
        Task<Product> CreatePorductAsync(ProductDto product);
    }
}
