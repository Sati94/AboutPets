using WebShopAPI.Model;
using WebShopAPI.Model.DTOS;

namespace WebShopAPI.Service.ProductServiceMap
{
    public interface IProductService
    {
        Task<Product> CreatePorductAsync(ProductDto product);
    }
}
