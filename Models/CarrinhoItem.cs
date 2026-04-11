using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerseEscalavel.Models
{
    public class CarrinhoItem
    {
        [Key] public int Id { get; set; }
        [Required] public Carrinho Carrinho { get; set; }
        [Required] public int IdProduto { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [Required] public decimal Quantidade { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [Required] public decimal Preco { get; set; }

    }
}
