using System.Text.Json.Serialization;
using WebShopAPI.Model.OrderModel.OrderStatus;
using WebShopAPI.Model.UserModels;
using WebShopAPI.Model.OrderModel.DeliveryType;

namespace WebShopAPI.Model.OrderModel
{
    public class Order
    {
        public int OrderId { get; set; }
        
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatuses OrderStatuses { get; set; }

        public string UserId { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public DeliveryTypes DeliveryType { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [JsonIgnore]

        public User User { get; set; }
    }
}
