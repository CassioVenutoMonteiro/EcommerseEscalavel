using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerseEscalavel.Models
{
    public class ItensComprado
    {
        [Key] public int Id { get; set; }

        [Required] public int IdProduto { get; set; }
        
        [ForeignKey("IdProduto")]
        [Required] public Produto Produto { get; set; }

        [Required] public Compra Compra { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required] public decimal Quantidade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required] public decimal Preco { get; set; }
    }
}
