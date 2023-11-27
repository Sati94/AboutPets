using System.Text.Json.Serialization;
using WebShopAPI.Model.OrderModel.OrderStatus;
using WebShopAPI.Model.UserModels;

namespace WebShopAPI.Model.OrderModel
{
    public class Order
    {
        public int OrderId { get; set; }
        
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatuses OrderStatuses { get; set; }

        public string UserId { get; set; }
        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        
        public User User { get; set; }
    }
}
