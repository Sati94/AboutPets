﻿namespace WebShopAPI.Service.Authentication
{
    public record AuthResult(
        bool Success,
        string? IdentityUserId,
        string Email,
        string UserName,
        string Token
        )
    {
        public readonly Dictionary<string, string> ErrorMessages = new Dictionary<string, string>();
    }
    
}
