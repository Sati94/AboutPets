using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using WebShopAPI.Model.OrderModel;

namespace WebShopAPI.Model.UserModels
{
    public class User : IdentityUser
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
       
        [JsonIgnore]     

        public List<OrderItem> OrderItems { get; set; }
        public List<Order> Orders { get; set; }
        public UserProfile Profile { get; set; }
    }
}
