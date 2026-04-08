using EcommerseEscalavel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerseEscalavel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FavoritesController(AppDbContext context)
        {
            _context = context;
        }

        // Adicionar produto aos favoritos
        [HttpPost]
        public async Task<IActionResult> AddFavorite(int usuarioId, int productId)
        {
            var favorite = new Favorite
            {
                UsuarioId = usuarioId,
                ProductId = productId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok(favorite);
        }

        // Listar favoritos do usuário
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> GetFavorites(int usuarioId)
        {
            var favorites = await _context.Favorites
                .Include(f => f.Product)
                .Where(f => f.UsuarioId == usuarioId)
                .ToListAsync();

            return Ok(favorites);
        }

        // Remover favorito
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFavorite(int id)
        {
            var favorite = await _context.Favorites.FindAsync(id);

            if (favorite == null)
                return NotFound();

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}