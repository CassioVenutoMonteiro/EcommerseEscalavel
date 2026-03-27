using System.ComponentModel.DataAnnotations;

namespace EcommerseEscalavel.Models
{
    // Classe auxiliar de Usuarios para manipular os dados da senha sem problemas
    public class UsuarioDto
    {
        public int? Id { get; set; }
        [Required] public string Nome { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
        [Required] public Perfil Perfil { get; set; }
    }
}
