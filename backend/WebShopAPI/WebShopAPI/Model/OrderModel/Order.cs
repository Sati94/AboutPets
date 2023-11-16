using WebShopAPI.Model.OrderModel.OrderStatus;
using WebShopAPI.Model.UserModels;

namespace WebShopAPI.Model.OrderModel
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatuses OrderStatuses { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public User User { get; set; }
    }
}
