using EcommerseEscalavel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security;
using System.Security.Claims;

namespace EcommerseEscalavel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhoController : ControllerBase
    {
        // Constante utilizada para chamar a conexão com o banco de dados
        private readonly AppDbContext _context;

        // Emcapsulamento do _context privado para que os metodos possam utiliza-lo normalmente
        public CarrinhoController(AppDbContext context)
        {
            _context = context;
        }

        // Get: Verbo utilizado para retorno de dados
        // Put: Verbo utilizado para alteração de dados
        // Post: Verbo utilizado para adição de dados
        // Delete: Verbo utilizado para exclusão de dados
        // [Verbo{var}]: Variaveis utilizadas através do link da API
        [Authorize]
        [HttpGet]                   // função utilizada para retornar todos os dados
        public async Task<ActionResult> GetAll()
        {
            var model = await _context.Carrinhos.Include(i => i.Itens).ToListAsync();
            // Ok status 200: Utilizado quando uma requisição foi bem sucedida e com returno
            return Ok(model);
        }

        [Authorize]
        [HttpPost]                   // função utilizada para adicionar dados
        public async Task<ActionResult> Create(CarrinhoItemDto model)
        {
            // Retorna o id do usuário
            string criadorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verifica se ja existe um carrinho em nome do usuário, caso não tem, é criado
            var carrinho = await _context.Carrinhos.FirstOrDefaultAsync(c => c.IdUsuario == int.Parse(criadorIdClaim));
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == model.IdProduto);

            if (produto == null) return NotFound("Produto não existe");

            if (carrinho == null)
            {
                Carrinho novoCarrinho = new Carrinho();
                novoCarrinho.IdUsuario = int.Parse(criadorIdClaim);

                _context.Carrinhos.Add(novoCarrinho);
                await _context.SaveChangesAsync();

                carrinho = await _context.Carrinhos.FirstOrDefaultAsync(c => c.IdUsuario == int.Parse(criadorIdClaim));
            }

            CarrinhoItem novoCarrinhoItem = new CarrinhoItem();
            //Vincula os produtos ao carrinho
            novoCarrinhoItem.Carrinho = carrinho;
            novoCarrinhoItem.IdProduto = produto.Id;
            novoCarrinhoItem.Preco = produto.Preco;
            novoCarrinhoItem.Quantidade = model.Quantidade; 

            // Cria no banco de dados o usuário adicionado
            _context.CarrinhoItens.Add(novoCarrinhoItem);
            // Informa para o banco de dados salvar as alterações
            await _context.SaveChangesAsync();

            // CreatedAtAction status 201: Utilizado quando uma requisição de inserção de dados foi bem sucedida retornando os dados inseridos
            return CreatedAtAction("GetById", new { id = novoCarrinhoItem.Id }, novoCarrinhoItem); // Para que o "GetById" funcione, é necessário que a mesma esteja criado ↓
        }

        [Authorize]
        [HttpGet("{id}")]            // função utilizada para retornar os dados com o email especificado
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Carrinhos.Include(i => i.Itens).FirstOrDefaultAsync(c => c.Id == id);

            if (model == null) return NotFound();

            return Ok(model);
        }

        // Explicação completa da função
        // Essa função é feito para atualizar o carrinho.
        // Ela tanto atualiza o preço assim que é chamada quanto altera a quantidade do mesmo item no carrinho
        // Como não é possivel possuir zero de um item no carrinho, é entendido que deseja apenas atualizar o preço do produto sem alterar a quantidade no caso da quantidade vir 0
        // Em casos que a quantidade vem maior que 0, a quantidade é atualizada para o valor informado
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] decimal quantidade)
        {
            // Retorna o usuário logado
            string criadorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Localiza o usuário com o e-mail informado
            var modeloDb = await _context.CarrinhoItens.FirstOrDefaultAsync(c => c.Id == id);

            // verifica se o item do carrinho existe
            if (modeloDb == null) return NotFound("Carrinho não existe");

            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == modeloDb.IdProduto);

            bool noContent;

            if(modeloDb.Preco == produto.Preco)
            {
                noContent = true;
            }
            else
            {
                noContent = false;
                modeloDb.Preco = produto.Preco;
            }

            if(quantidade > 0)
            {
                modeloDb.Quantidade = quantidade;
            }

            _context.CarrinhoItens.Update(modeloDb);
            await _context.SaveChangesAsync();

            if (noContent)
            {
                return NoContent();
            }
            else
            {
                return Ok("Preço do produto atualizado");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            // Retorna o usuário logado
            string criadorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Localiza as informações do e-mail do usuário
            var model = await _context.CarrinhoItens.FirstOrDefaultAsync(p => p.Id == id);

            if (model == null) return NotFound("Produto não encontrado");
            
            _context.CarrinhoItens.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
