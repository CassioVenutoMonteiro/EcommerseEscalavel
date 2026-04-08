using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerseEscalavel.Models;

namespace EcommerseEscalavel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Comprar(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
                return BadRequest("Produto não encontrado");

            var order = new Order
            {
                CreatedAt = DateTime.Now,
                Items = new List<OrderItem>()
            };

            var item = new OrderItem
            {
                ProductId = productId,
                Quantity = 1,
                Price = product.Price
            };

            order.Items.Add(item);

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();

            return Ok(orders);
        }
    }
}