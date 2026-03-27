using System.ComponentModel.DataAnnotations;

namespace EcommerseEscalavel.Models
{
    public class AuthenticateDto
    {
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
    }
}
