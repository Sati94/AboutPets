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
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string ImageBase64 { get; set; }
        public Category GetCategory()
        {
            return (Category)CategoryId;
        }

        
        public SubCategory GetSubCategory()
        {
            return (SubCategory)SubCategoryId;
        }
    }
}
