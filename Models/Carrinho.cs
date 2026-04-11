using System.ComponentModel.DataAnnotations;

namespace EcommerseEscalavel.Models
{
    public class Carrinho
    {
        [Key] public int Id { get; set; }
        [Required] public int IdUsuario { get; set; }
        public ICollection<CarrinhoItem> Itens { get; set; }
    }
}
