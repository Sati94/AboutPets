using WebShopAPI.Model;

namespace WebShopAPI.Service.ProductServiceMap
{
    public interface IProductService
    {
        Task<Product> CreatePorductAsync(Product product);
    }
}
