using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerseEscalavel.Models
{
    // Classe Usuario
    public class Usuario
    {
        // [Key] indica que é a chave primaria para a criação automatica no banco de dados
        // [Rquired] indica que é obrigatório o preenchimento para a cri~ção automatica no banco de dados
        // [JsonIgnore] indica que não deve mostrar nem manipular este campo, evitando mostrar a senha para usuarios
        // [Display(Name = "")] Indica a maneira correta do sistema gerar os dados para evitar a utilização do plural incorreta
        [Key] public int Id { get; set; }
        [Required] public string Nome { get; set; }
        [Required] public string Email { get; set; }
        [Required][JsonIgnore] public string Password { get; set; }
        [Required] public Perfil Perfil { get; set; }

        // informações de controle LGPD
        [Required] public DateTime DataCriacao { get; set; }
         public int? UsuarioCriacao { get; set; }
        [Required] public DateTime DataAlteracao { get; set; }
         public int? UsuarioAlteracao { get; set; }
        [Required] public bool Ativo { get; set; }
    }

    // Array criado para preenchimento predefinido. O primeiro dado é representado por 0, o segundo como 1 e assim por diante
    public enum Perfil
    {
        [Display(Name = "Administrador")] Administrador,    //0
        [Display(Name = "Usuario")] Usuario,                //1
    }
}
