using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application;
using ProductCatalog.Infrastructure;
using ProductCatalog.Infrastructure.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// --- 0. CONFIGURAÇÃO DO SERVIÇO DE BANCO DE DADOS (LEITURA DE SECRET) ---
// Se um secret Docker for montado em /run/secrets/db_password, compomos a connection string em memória
var inMemoryConfig = new Dictionary<string, string>();
var secretPath = "/run/secrets/db_password";
if (File.Exists(secretPath))
{
    var dbPassword = File.ReadAllText(secretPath).Trim();
    var host = builder.Configuration["SQL_SERVER__HOST"] ?? "sqlserver";
    var db = builder.Configuration["SQL_SERVER__DATABASE"] ?? "ProductCatalogDb";
    var user = builder.Configuration["SQL_SERVER__USER"] ?? "sa";
    inMemoryConfig["ConnectionStrings:DefaultConnection"] =
        $"Server={host},1433;Database={db};User Id={user};Password={dbPassword};TrustServerCertificate=True;";
}

// Injeta a configuração em memória (vai sobrescrever appsettings / env vars se presentes)
var inMemoryForConfig = inMemoryConfig.Select(kv => new KeyValuePair<string, string?>(kv.Key, kv.Value));
builder.Configuration.AddInMemoryCollection(inMemoryForConfig);


// --- 1. CONFIGURAÇÃO DA INJEÇÃO DE DEPENDÊNCIA (DI) ---

// Adiciona os serviços da camada Application (Services, AutoMapper, FluentValidation)
builder.Services.AddApplication();

// Adiciona os serviços da camada Infrastructure (DbContext, Repositório)
builder.Services.AddInfrastructure(builder.Configuration);

// Configuração do FluentValidation para interceptar modelos antes da action
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation(options =>
{
    options.DisableDataAnnotationsValidation = true;
});
builder.Services.AddFluentValidationClientsideAdapters();


// --- 2. CONFIGURAÇÕES ADICIONAIS ---

// Adiciona serviços para o Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// --- 3. BUILD DA APLICAÇÃO E PIPELINE ---
var app = builder.Build();

// Aplica as migrações pendentes ao iniciar a aplicação
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configuração do pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();