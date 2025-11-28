using System.Collections.Generic;
using System.Threading.Tasks;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Tests.TestDoubles
{
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _list = new List<Product>();

        public Task AddAsync(Product product)
        {
            product.Id = _list.Count + 1;
            _list.Add(product);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Product product)
        {
            _list.Remove(product);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Product>> GetAllAsync(string? nomeBusca, string? ordenarPor, string? direcao)
        {
            return Task.FromResult<IEnumerable<Product>>(_list);
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            return Task.FromResult(_list.Find(p => p.Id == id));
        }

        public Task UpdateAsync(Product product)
        {
            var idx = _list.FindIndex(p => p.Id == product.Id);
            if (idx >= 0) _list[idx] = product;
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
