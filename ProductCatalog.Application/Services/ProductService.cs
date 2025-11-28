using System;
using System.Collections.Generic;
using AutoMapper;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Application.Services
{
    public class ProductService(IProductRepository repository, IMapper mapper, ICacheService cache) : IProductService
    {
        #region Variables
        private readonly IProductRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly ICacheService _cache = cache;
        #endregion

        #region Constants
        private static readonly TimeSpan ProductsCacheDuration = TimeSpan.FromMinutes(7);
        #endregion

        #region Methods
        public async Task<ProductResponseDTO> AddAsync(ProductCreateUpdateDTO dto)
        {
            var entity = _mapper.Map<Product>(dto);
            entity.ValidatePrice();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            await _cache.SetStringAsync("products:version", Guid.NewGuid().ToString());
            return _mapper.Map<ProductResponseDTO>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Produto não encontrado.");
            await _repository.DeleteAsync(existing);
            await _repository.SaveChangesAsync();
            await _cache.SetStringAsync("products:version", Guid.NewGuid().ToString());
        }

        public async Task<IEnumerable<ProductResponseDTO>> GetAllAsync(string? nomeBusca, string? ordenarPor, string? direcao)
        {
            var version = await _cache.GetStringAsync("products:version");
            if (string.IsNullOrEmpty(version))
            {
                version = Guid.NewGuid().ToString();
                await _cache.SetStringAsync("products:version", version);
            }

            var key = $"products:list:v{version}:q={nomeBusca ?? ""}:o={ordenarPor ?? ""}:d={direcao ?? ""}";

            var cached = await _cache.GetAsync<IEnumerable<ProductResponseDTO>>(key);
            if (cached != null) return cached;

            var list = await _repository.GetAllAsync(nomeBusca, ordenarPor, direcao);
            var dtoList = _mapper.Map<IEnumerable<ProductResponseDTO>>(list);
            await _cache.SetAsync(key, dtoList, ProductsCacheDuration);
            return dtoList;
        }

        public async Task<ProductResponseDTO?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ProductResponseDTO>(entity);
        }

        public async Task UpdateAsync(int id, ProductCreateUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Produto não encontrado.");

            existing.Name = dto.Name;
            existing.Stock = dto.Stock;
            existing.Price = dto.Price;
            existing.ValidatePrice();

            await _repository.UpdateAsync(existing);
            await _repository.SaveChangesAsync();
            await _cache.SetStringAsync("products:version", Guid.NewGuid().ToString());
        }
        #endregion
    }
}
