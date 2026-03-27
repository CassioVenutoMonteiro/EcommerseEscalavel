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
    }
}
