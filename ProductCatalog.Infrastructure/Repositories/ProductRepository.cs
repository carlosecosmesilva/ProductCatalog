using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class ProductRepository(ApplicationDbContext context) : IProductRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product Product)
        {
            await _context.Products.AddAsync(Product);
        }

        public Task UpdateAsync(Product Product)
        {
            _context.Products.Update(Product);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Product Product)
        {
            _context.Products.Remove(Product);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        // --- BUSCA E ORDENAÇÃO (REQUISITO FUNCIONAL) ---

        public async Task<IEnumerable<Product>> GetAllAsync(string? nomeBusca, string? ordenarPor, string? direcao)
        {
            IQueryable<Product> query = _context.Products.AsNoTracking();

            // 1. Filtragem (Busca Product pelo nome)
            if (!string.IsNullOrEmpty(nomeBusca))
            {
                query = query.Where(p => p.Name.Contains(nomeBusca));
            }

            // 2. Ordenação (Ordenar os Products por diferentes campos)
            if (!string.IsNullOrEmpty(ordenarPor))
            {
                string direcaoLower = direcao?.ToLower() ?? "asc";
                bool isAscending = direcaoLower == "asc";

                query = ordenarPor.ToLower() switch
                {
                    "nome" => isAscending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                    "estoque" => isAscending ? query.OrderBy(p => p.Stock) : query.OrderByDescending(p => p.Stock),
                    "valor" => isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                    _ => query.OrderBy(p => p.Id)
                };
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }

            // 3. Executa a query no banco de dados
            return await query.ToListAsync();
        }
    }
}
