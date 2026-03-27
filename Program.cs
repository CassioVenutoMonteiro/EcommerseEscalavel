using EcommerseEscalavel.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Utilizado para evitar que ciclos no codigo trave a API, por exemplo:
// Um veiculo possui usuário e usuário possui veiculo. Ao apresentar os dados, veiculo chama usuário e usuário chama veiculo.
// Essa linha impede no primeiro ciclo e não apresenta dados repetidos
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Este comando realiza a conexão com o banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("defaltConnection")));

// O bloco a seguir, é utilizado para gestão de token que o usuário recebe para acessar aos comandos permitidos pelo tipo de usuário
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})// Adding Jwt Bearer
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,                                          // Senha de 32 caracteres para ser utilizado como chave de geração do Token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("x7bZGKD4Hae7o01WXN3Zhlp6oyVDFWoP"))
        };
    });

// builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Utilizado para informar ao sistema que será necessário a realização de login para que tenha acesso aos comandos expecificos da API
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
