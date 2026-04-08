namespace EcommerseEscalavel.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Nota { get; set; }

        public string Comentario { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}