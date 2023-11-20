using System.ComponentModel.DataAnnotations;

namespace WebShopAPI.Contracts
{
    public record RegistrationRequest(
        [Required] string Email,
        [Required] string UserName,
        [Required] string Password);
    
        
    
}
