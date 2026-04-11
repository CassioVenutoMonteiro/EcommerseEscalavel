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
    public class EstoqueController : ControllerBase
    {
        // Constante utilizada para chamar a conexão com o banco de dados
        private readonly AppDbContext _context;

        // Emcapsulamento do _context privado para que os metodos possam utiliza-lo normalmente
        public EstoqueController(AppDbContext context)
        {
            _context = context;
        }

        // Get: Verbo utilizado para retorno de dados
        // Put: Verbo utilizado para alteração de dados
        // Post: Verbo utilizado para adição de dados
        // Delete: Verbo utilizado para exclusão de dados
        // [Verbo{var}]: Variaveis utilizadas através do link da API

        [HttpGet]                   // função utilizada para retornar todos os dados
        public async Task<ActionResult> GetAll()
        {
            var model = await _context.Produtos.ToListAsync();
            // Ok status 200: Utilizado quando uma requisição foi bem sucedida e com returno
            return Ok(model);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]                   // função utilizada para adicionar dados
        public async Task<ActionResult> Create(Produto model)
        {
            // Verifica se o usuário que está tentando cadastrar ja existe no banco de dados, retornando true ou false
            bool produtoExiste = await _context.Produtos.AnyAsync(p => p.Nome == model.Nome);

            if (produtoExiste)
            {
                // Conflict status 409: Utilizado quando ocorre um conflito como inserção de usuário já existente
                return Conflict("Este produto já existe");
            }
            
            string criadorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Campos de controle LGPD
            model.DataCriacao = DateTime.UtcNow;
            model.UsuarioCriacao = (criadorIdClaim != null) ? int.Parse(criadorIdClaim) : null;
            model.DataAlteracao = DateTime.UtcNow;
            model.UsuarioAlteracao = (criadorIdClaim != null) ? int.Parse(criadorIdClaim) : null;
            model.Ativo = true;

            // Cria no banco de dados o usuário adicionado
            _context.Produtos.Add(model);
            // Informa para o banco de dados salvar as alterações
            await _context.SaveChangesAsync();

            // CreatedAtAction status 201: Utilizado quando uma requisição de inserção de dados foi bem sucedida retornando os dados inseridos
            return CreatedAtAction("GetById", new { id = model.Id }, model); // Para que o "GetById" funcione, é necessário que a mesma esteja criado ↓
        }

        [HttpGet("{id}")]            // função utilizada para retornar os dados com o email especificado
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);

            if (model == null) return NotFound();

            return Ok(model);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]           // função utilizada para atualizar os dados com o email especificado
        public async Task<ActionResult> Update(int id, Produto model)
        {
            // Retorna o usuário logado
            string criadorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Localiza o usuário com o e-mail informado
            var modeloDb = await _context.Produtos.FirstOrDefaultAsync(c => c.Id == id);

            // verifica se o usuário do e-mail informado existe
            if (modeloDb == null) return NotFound("Produto não existe");

            modeloDb.Nome = model.Nome;
            modeloDb.Descricao = model.Descricao;
            modeloDb.Preco = model.Preco;
            modeloDb.QuantEstoque = model.QuantEstoque;
            modeloDb.Imagem = model.Imagem;
            modeloDb.DataAlteracao = DateTime.UtcNow;
            modeloDb.UsuarioAlteracao = (criadorIdClaim != null) ? int.Parse(criadorIdClaim) : null;

            _context.Produtos.Update(modeloDb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            // Retorna o usuário logado
            string criadorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Localiza as informações do e-mail do usuário
            var model = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);

            if (model == null) return NotFound("Produto não encontrado");

            //Desabilita o produto ao invés de excluir para conformidade com a LGPD
            model.Ativo = false;

            // Campos da LGPD
            model.DataAlteracao = DateTime.UtcNow;
            model.UsuarioAlteracao = int.Parse(criadorIdClaim);

            _context.Produtos.Update(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
