using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application;
using ProductCatalog.Infrastructure;
using ProductCatalog.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 0. CONFIGURAÇÃO DO SERVIÇO DE BANCO DE DADOS ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// --- 1. CONFIGURAÇÃO DA INJEÇÃO DE DEPENDÊNCIA (DI) ---

// Adiciona os serviços da camada Application (Services, AutoMapper, FluentValidation)
builder.Services.AddApplication();

// Adiciona os serviços da camada Infrastructure (DbContext, Repositório)
builder.Services.AddInfrastructure(builder.Configuration);

// Configuração do FluentValidation para interceptar modelos antes da action
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.DisableDataAnnotationsValidation = true;
        fv.AutomaticValidationEnabled = true;
    });


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