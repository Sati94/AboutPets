using System.Text.Json.Serialization;

namespace WebShopAPI.Model.UserModels
{
    public class UserProfile
    {
        public int UserProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public decimal Bonus { get; set; }

        public string UserId { get; set; }
         [JsonIgnore]
        public User User { get; set; }
    }
}
