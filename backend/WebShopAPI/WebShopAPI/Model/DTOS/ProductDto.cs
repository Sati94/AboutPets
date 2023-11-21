using WebShopAPI.Model.CategoryClasses;

namespace WebShopAPI.Model.DTOS
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal Discount { get; set; }
        public Category Category { get; set; }
        public SubCategory SubCategory { get; set; }
        public string ImageBase64 { get; set; }
    }
}
