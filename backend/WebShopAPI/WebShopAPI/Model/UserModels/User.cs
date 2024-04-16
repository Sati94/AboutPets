using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using WebShopAPI.Model.OrderModel;


namespace WebShopAPI.Model.UserModels
{
    public class User : IdentityUser
    { 
       
        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        [JsonIgnore]
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public UserProfile Profile { get; set; }

        
        public User()
        {
            Id = Guid.NewGuid().ToString();
          
        }
    }
    
}
