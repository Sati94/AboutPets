using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using WebShopAPI.Model.OrderModel;

namespace WebShopAPI.Model.UserModels
{
    public class User : IdentityUser
    { 
        public string UserId { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public List<Order> Orders { get; set; }
        public UserProfile Profile { get; set; }
        
        public User()
        {
            UserId = Guid.NewGuid().ToString();
        }
    }
    
}
