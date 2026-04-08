using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerseEscalavel.Models;

namespace EcommerseEscalavel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        // RF-004 - Criar avaliação
        [HttpPost]
        public async Task<IActionResult> CriarReview(int usuarioId, int productId, int nota, string comentario)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            var produto = await _context.Products.FindAsync(productId);

            if (usuario == null || produto == null)
                return BadRequest("Usuário ou produto não encontrado");

            var review = new Review
            {
                UsuarioId = usuarioId,
                ProductId = productId,
                Nota = nota,
                Comentario = comentario,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);

            await _context.SaveChangesAsync();

            return Ok(review);
        }

        // Listar avaliações
        [HttpGet]
        public async Task<IActionResult> ListarReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Usuario)
                .Include(r => r.Product)
                .ToListAsync();

            return Ok(reviews);
        }

        // Listar avaliações de um jogo
        [HttpGet("produto/{productId}")]
        public async Task<IActionResult> ReviewsDoProduto(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.Usuario)
                .ToListAsync();

            return Ok(reviews);
        }
    }
}