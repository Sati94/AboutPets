using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using WebShopAPI.Model.OrderModel;

namespace WebShopAPI.Model.UserModels
{
    public class User 
    { 
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string IdentityUserId { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<Order> Orders { get; set; }
        public UserProfile Profile { get; set; }
        [JsonIgnore]
        public IdentityUser IdentityUser { get; set; }
        
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
    
}
