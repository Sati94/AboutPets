namespace WebShopAPI.Service.Authentication
{
    public record AuthResult(
        bool Success,
        string? IdentityUserId,
        string Email,
        string UserName,
        string Token,
        string Role
   
        )
    {
        public readonly Dictionary<string, string> ErrorMessages = new Dictionary<string, string>();
    }
    
}
