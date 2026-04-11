using EcommerseEscalavel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerseEscalavel.Controllers
{
    [Route("api/admin/reports")]
    [ApiController]
    public class RelatorioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RelatorioController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport()
        {
            var totalCompras = await _context.Compras.CountAsync();

            var totalItems = await _context.ItensComprados
                .SumAsync(oi => oi.Quantidade);

            var totalRevenue = await _context.ItensComprados
                .SumAsync(oi => oi.Quantidade * oi.Preco);

            var topProduto = await _context.ItensComprados
                .Include(oi => oi.Produto)
                .GroupBy(oi => new { oi.IdProduto, oi.Produto.Nome })
                .Select(g => new
                {
                    ProdutoId = g.Key.IdProduto,
                    Name = g.Key.Nome,
                    TotalSold = g.Sum(x => x.Quantidade)
                })
                .OrderByDescending(x => x.TotalSold)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                totalCompras,
                totalItems,
                totalRevenue,
                topProduto
            });
        }
    }
}