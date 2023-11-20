using System.ComponentModel.DataAnnotations;

namespace WebShopAPI.Contracts
{
    public record RegistrationResponse(
        [Required] string Email,
        [Required] string UserName);
    
    
}
