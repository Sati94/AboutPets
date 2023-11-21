namespace WebShopAPI.Model.UserModels
{
    public class UserProfile
    {
        public int UserProfileId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal Bonus { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
    }
}
