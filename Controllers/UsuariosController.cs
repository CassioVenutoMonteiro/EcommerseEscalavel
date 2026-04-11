using EcommerseEscalavel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcommerseEscalavel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        // Constante utilizada para chamar a conexão com o banco de dados
        private readonly AppDbContext _context;

        // Emcapsulamento do _context privado para que os metodos possam utiliza-lo normalmente
        public UsuariosController(AppDbContext context)
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
            var model = await _context.Usuarios.ToListAsync();
            // Ok status 200: Utilizado quando uma requisição foi bem sucedida e com returno
            return Ok(model);
        }

        [HttpPost]                   // função utilizada para adicionar dados
        public async Task<ActionResult> Create(UsuarioDto model)
        {
            // Verifica se o usuário que está tentando cadastrar ja existe no banco de dados, retornando true ou false
            bool usuarioExiste = await _context.Usuarios.AnyAsync(u => u.Email == model.Email);

            if (usuarioExiste)
            {
                // Conflict status 409: Utilizado quando ocorre um conflito como inserção de usuário já existente
                return Conflict("Este usuário já existe");
            }

            // Verifica se o usuario está logado ou não e qual o seu perfil
            bool estaLogado = User.Identity.IsAuthenticated;
            string roleLogado = User.FindFirst(ClaimTypes.Role)?.Value;
            string criadorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verifica se a conta a ser criada é de Administrador ou não
            if (model.Perfil == Perfil.Administrador)
            {
                // Verifica se o usuário tem permissão de admin para criar uma conta de admin
                if (!estaLogado || roleLogado != "Administrador")
                {
                    return StatusCode(403, "Apenas administradores podem criar novos administradores.");
                }
            }

            // Instacia da classe Usuario através do dados da classe UsuarioDto devido ao campo senha estar invisivel com [JsonIgnore]
            Usuario novo = new Usuario()
            {
                Nome = model.Nome,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Perfil = model.Perfil,
                DataCriacao = DateTime.UtcNow,
                UsuarioCriacao = (criadorIdClaim != null) ? int.Parse(criadorIdClaim) : null,
                DataAlteracao = DateTime.UtcNow,
                UsuarioAlteracao = (criadorIdClaim != null) ? int.Parse(criadorIdClaim) : null,
                Ativo = true
            };

            // Cria no banco de dados o usuário adicionado
            _context.Usuarios.Add(novo);
            // Informa para o banco de dados salvar as alterações
            await _context.SaveChangesAsync();

            // CreatedAtAction status 201: Utilizado quando uma requisição de inserção de dados foi bem sucedida retornando os dados inseridos
            return CreatedAtAction("GetById", new { id = novo.Id }, novo); // Para que o "GetById" funcione, é necessário que a mesma esteja criado ↓
        }

        [HttpGet("{id}")]            // função utilizada para retornar os dados com o email especificado
        public async Task<ActionResult> GetById(int id)
        {
            var model = await _context.Usuarios.FirstOrDefaultAsync(c => c.Id == id);

            if (model == null) return NotFound();

            return Ok(model);
        }

        [Authorize]
        [HttpPut("{id}")]           // função utilizada para atualizar os dados com o email especificado
        public async Task<ActionResult> Update(int id, UsuarioDto model)
        {
            // Verifica se o usuario está logado ou não e qual o seu perfil
            bool estaLogado = User.Identity.IsAuthenticated;
            string roleLogado = User.FindFirst(ClaimTypes.Role)?.Value;
            string criadorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //Console.WriteLine("criadorIdClaim: " + criadorIdClaim);

            // Localiza o usuário com o e-mail informado
            var modeloDb = await _context.Usuarios.FirstOrDefaultAsync(c => c.Id == id);

            // verifica se o usuário do e-mail informado existe
            if (modeloDb == null) return NotFound("Usuario não existe");

            Console.WriteLine("model.Id: " + modeloDb.Id + "int.Parse(criadorIdClaim): " + int.Parse(criadorIdClaim));
            // verifica o usuário informado é admin
            if (model.Perfil == Perfil.Administrador)
            {
                // Verifica se o usuário tem permissão de admin para criar uma conta de admin
                if (!estaLogado || roleLogado != "Administrador")
                {
                    return StatusCode(403, "Apenas administradores podem alterar administradores.");
                }
            }
            else if (model.Perfil == Perfil.Usuario) // verifica se é o próprio dono da conta quem está alterando
            {
                if (modeloDb.Id != int.Parse(criadorIdClaim))
                {
                    return StatusCode(403, "Apenas o dono da conta pode alterar a própria conta.");
                }
            }

            Usuario novo = new Usuario()
            {
                Nome = model.Nome,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Perfil = model.Perfil,
                DataAlteracao = DateTime.UtcNow,
                UsuarioAlteracao = (criadorIdClaim != null) ? int.Parse(criadorIdClaim) : null,
            };

            //Dados a serem alterados
            modeloDb.Nome = novo.Nome;
            modeloDb.Email = novo.Email;
            modeloDb.Password = novo.Password;
            modeloDb.Perfil = novo.Perfil;
            modeloDb.DataAlteracao = novo.DataAlteracao;
            modeloDb.UsuarioAlteracao = novo.UsuarioAlteracao;

            _context.Usuarios.Update(modeloDb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            // Verifica se o usuario está logado ou não e qual o seu perfil
            bool estaLogado = User.Identity.IsAuthenticated;
            string roleLogado = User.FindFirst(ClaimTypes.Role)?.Value;
            string criadorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Localiza as informações do e-mail do usuário
            var model = await _context.Usuarios.FirstOrDefaultAsync(c => c.Id == id);

            if (model == null) return NotFound("Usuário não encontrado");

            // Verifica se o usuário deletado é administrador
            if (model.Perfil == Perfil.Administrador)
            {
                // Verifica se o usuário tem permissão de admin para criar uma conta de admin
                if (!estaLogado || roleLogado != "Administrador")
                {
                    return StatusCode(403, "Apenas administradores podem excluir administradores.");
                }
            }
            else if(model.Perfil == Perfil.Usuario) // verifica se é o próprio dono da conta quem está excluindo
            {
                if (model.Id != int.Parse(criadorIdClaim))
                {
                    return StatusCode(403, "Apenas o dono da conta pode excluir a própria conta.");
                }
            }

            // Caso não seja administrador, o mesmo poderá ser deletado apenas pelo proprio dono da conta

            //Desabilita o usuário ao invés de excluir para conformidade com a LGPD
            model.Ativo = false;

            // Campos da LGPD
            model.DataAlteracao = DateTime.UtcNow;
            model.UsuarioAlteracao = int.Parse(criadorIdClaim);

            _context.Usuarios.Update(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Autenticação: Exclusivos de usuarios
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult> Authenticate(AuthenticateDto model)
        {
            var usuarioDb = await _context.Usuarios.FirstOrDefaultAsync(c => c.Email == model.Email);

            if (usuarioDb == null || !BCrypt.Net.BCrypt.Verify(model.Password, usuarioDb.Password)) return Unauthorized("Usuario ou senha invalido");

            var jwt = GenerateJwtToken(usuarioDb);

            return Ok(new { jwtToken = jwt });
        }

        private string GenerateJwtToken(Usuario model)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("x7bZGKD4Hae7o01WXN3Zhlp6oyVDFWoP");
            var Claims = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, model.Id.ToString()),
                new Claim(ClaimTypes.Role, model.Perfil.ToString())
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = Claims,
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
