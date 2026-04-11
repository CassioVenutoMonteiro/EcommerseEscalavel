using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerseEscalavel.Models
{
    public class Produto
    {
        [Key] public int Id { get; set; }
        [Required] public string Nome { get; set; }
        [Required] public string Descricao { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        [Required] public decimal Preco { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required] public decimal QuantEstoque { get; set; }
        public string Imagem { get; set; }

        // informações de controle LGPD
        [Required] public DateTime DataCriacao { get; set; }
        public int? UsuarioCriacao { get; set; }
        [Required] public DateTime DataAlteracao { get; set; }
        public int? UsuarioAlteracao { get; set; }
        [Required] public bool Ativo { get; set; }
    }
}
