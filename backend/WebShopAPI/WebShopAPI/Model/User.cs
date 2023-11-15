using WebShopAPI.Model.OrderModel;

namespace WebShopAPI.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }  
        public string Password { get; set; }
        public string Address { get; set; }
        public decimal Bonus { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        public List<Order> Orders { get; set; }
    }
}
