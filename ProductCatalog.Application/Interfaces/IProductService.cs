using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDTO> AddAsync(ProductCreateUpdateDTO dto);
        Task UpdateAsync(int id, ProductCreateUpdateDTO dto);
        Task DeleteAsync(int id);
        Task<ProductResponseDTO?> GetByIdAsync(int id);
        Task<IEnumerable<ProductResponseDTO>> GetAllAsync(string? nomeBusca, string? ordenarPor, string? direcao);
    }
}
