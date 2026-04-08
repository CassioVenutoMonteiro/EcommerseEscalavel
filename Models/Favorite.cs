namespace EcommerseEscalavel.Models
{
    public class Favorite
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
