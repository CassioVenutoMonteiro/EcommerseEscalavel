using EcommerseEscalavel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerseEscalavel.Controllers
{
    [Route("api/admin/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport()
        {
            var totalOrders = await _context.Orders.CountAsync();

            var totalItems = await _context.OrderItems
                .SumAsync(oi => oi.Quantity);

            var totalRevenue = await _context.OrderItems
                .SumAsync(oi => oi.Quantity * oi.Price);

            var topProduct = await _context.OrderItems
                .Include(oi => oi.Product)
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    Name = g.Key.Name,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .FirstOrDefaultAsync();

            return Ok(new
            {
                totalOrders,
                totalItems,
                totalRevenue,
                topProduct
            });
        }
    }
}