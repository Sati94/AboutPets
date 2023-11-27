using WebShopAPI.Model;

namespace WebShopAPI.Service.ProductServiceMap
{
    public class DiscountCalculator
    {
        public decimal CalculateDiscountedPrice(Product product)
        {
            return product.Discount > 0 ? product.Price - (product.Price * product.Discount / 100) : product.Price;
        }
    }
}
