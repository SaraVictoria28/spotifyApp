using AppSpotify.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.

builder.Services.AddDbContext<SpotifyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConexaoPadrao"))
);

// Configura controladores e serialização JSON para evitar ciclos de referência.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
    );

// Configura CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin() // Permite requisições de qualquer origem (muito aberto para produção)
              .AllowAnyMethod()  // Permite qualquer método HTTP (GET, POST, PUT, DELETE, etc.)
              .AllowAnyHeader(); // Permite qualquer cabeçalho na requisição
    });
});

// Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline de requisição HTTP.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilita servir arquivos estáticos (necessário para acessar as pastas /musicas e /images)
app.UseStaticFiles();

// AQUI: Aplica a política de CORS definida. ESSENCIAL para permitir requisições cross-origin.
app.UseCors("PermitirTudo");

app.UseAuthorization();

app.MapControllers();

app.Run();