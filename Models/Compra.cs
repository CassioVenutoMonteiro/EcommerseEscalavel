using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerseEscalavel.Models
{
    public class Compra
    {
        [Key] public int Id { get; set; }
        [Required] public int IdUsuario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required] public decimal Total { get; set; }
        [Required] public DateTime Data { get; set; }

        [Required] public ICollection<ItensComprado> Itens { get; set; }
    }
}