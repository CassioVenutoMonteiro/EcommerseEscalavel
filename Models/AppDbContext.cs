using Microsoft.EntityFrameworkCore;

namespace EcommerseEscalavel.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) 
        { 
            
        }

        // Metodo criado para realizar conexões n para n para as tabelas
        protected override void OnModelCreating(ModelBuilder model)
        {
            
        }

        // Metodos criados para realizar o crud de cada model através do controller
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
