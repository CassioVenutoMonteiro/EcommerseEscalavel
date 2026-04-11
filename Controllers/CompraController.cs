using EcommerseEscalavel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerseEscalavel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraController : ControllerBase
    {
        // Constante utilizada para chamar a conexão com o banco de dados
        private readonly AppDbContext _context;

        // Emcapsulamento do _context privado para que os metodos possam utiliza-lo normalmente
        public CompraController(AppDbContext context)
        {
            _context = context;
        }

        // Get: Verbo utilizado para retorno de dados
        // Put: Verbo utilizado para alteração de dados
        // Post: Verbo utilizado para adição de dados
        // Delete: Verbo utilizado para exclusão de dados
        // [Verbo{var}]: Variaveis utilizadas através do link da API


        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var model = await _context.Compras.Include(i => i.Itens).ToListAsync();
            // Ok status 200: Utilizado quando uma requisição foi bem sucedida e com returno
            return Ok(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Compra()
        {
            string criadorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int usuarioId = int.Parse(criadorIdClaim);

            // 1. Busca o carrinho com os itens e os dados dos produtos já inclusos para evitar 200 selects no loop
            var carrinho = await _context.Carrinhos.Include(c => c.Itens).FirstOrDefaultAsync(c => c.IdUsuario == usuarioId);

            if (carrinho == null || !carrinho.Itens.Any())
                return NotFound("Carrinho vazio.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 2. Cria o cabeçalho da Compra (ainda não salvou no banco, está no contexto)
                var novaCompra = new Compra()
                {
                    IdUsuario = usuarioId,
                    Data = DateTime.Now,
                    Total = 0, // Vamos somar no loop
                    Itens = new List<ItensComprado>()
                };

                foreach (var item in carrinho.Itens)
                {
                    var produto = await _context.Produtos.FindAsync(item.IdProduto);

                    // Validações
                    if (produto == null) throw new Exception("Produto não encontrado.");
                    if (produto.QuantEstoque < item.Quantidade)
                        throw new Exception($"Estoque insuficiente: {produto.Nome}");

                    // 3. Baixa o estoque
                    produto.QuantEstoque -= item.Quantidade;

                    // 4. Cria o item da compra e vincula à 'novaCompra'
                    var itemComprado = new ItensComprado
                    {
                        IdProduto = item.IdProduto,
                        Quantidade = item.Quantidade,
                        Preco = produto.Preco,
                        Compra = novaCompra // O EF entende que este item pertence a essa compra
                    };

                    novaCompra.Total += (produto.Preco * item.Quantidade);
                    _context.ItensComprados.Add(itemComprado);
                }

                // 5. Adiciona a compra ao contexto
                _context.Compras.Add(novaCompra);

                // 6. Limpa o carrinho (já que a compra foi feita)
                _context.CarrinhoItens.RemoveRange(carrinho.Itens);

                // O SaveChanges vai enviar TUDO: baixa estoque, cria compra, cria itens e limpa carrinho.
                // Se qualquer um falhar, nada acontece.
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { CompraId = novaCompra.Id, Total = novaCompra.Total });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{id}")]            // função utilizada para retornar os dados com o email especificado
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Compras.Include(i => i.Itens).FirstOrDefaultAsync(c => c.Id == id);

            if (model == null) return NotFound();

            return Ok(model);
        }
    }
}
