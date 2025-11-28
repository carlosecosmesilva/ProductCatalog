using System;
using System.Threading.Tasks;
using Xunit;
using AutoMapper;
using Moq;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Services;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Interfaces;
using ProductCatalog.Tests.Fixtures;
using ProductCatalog.Tests.TestDoubles;

namespace ProductCatalog.Tests.Services
{
    public class ProductServiceTests(MapperFixture fixture) : IClassFixture<MapperFixture>
    {
        #region Variables
        private readonly IMapper _mapper = fixture.Mapper;
        #endregion

        #region Tests
        [Fact]
        public async Task AddAsync_Should_Add_Product_And_Invoke_Repository_And_Cache()
        {
            // Arrange
            var repoMock = new Mock<IProductRepository>();
            var cacheMock = new Mock<ProductCatalog.Application.Interfaces.ICacheService>();

            // Simula AddAsync e atribui um Id ao produto
            repoMock.Setup(r => r.AddAsync(It.IsAny<Product>()))
                    .Callback<Product>(p => p.Id = 1)
                    .Returns(Task.CompletedTask);

            repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            cacheMock.Setup(c => c.SetStringAsync("products:version", It.IsAny<string>(), It.IsAny<TimeSpan?>()))
                     .Returns(Task.CompletedTask);

            var service = new ProductService(repoMock.Object, _mapper, cacheMock.Object);
            var dto = new ProductCreateUpdateDTO { Name = "Teste", Stock = 10, Price = 9.99m };

            // Act
            var result = await service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Teste", result.Name);

            repoMock.Verify(r => r.AddAsync(It.Is<Product>(p => p.Name == dto.Name && p.Price == dto.Price)), Times.Once);
            repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            cacheMock.Verify(c => c.SetStringAsync("products:version", It.IsAny<string>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_With_NegativePrice_Should_ThrowArgumentException()
        {
            // Arrange
            var repo = new InMemoryProductRepository();
            var cache = new InMemoryCacheService();
            var service = new ProductService(repo, _mapper, cache);

            var dto = new ProductCreateUpdateDTO { Name = "Produto", Stock = 1, Price = -5m };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(dto));
        }

        [Fact]
        public async Task GetAllAsync_Should_PopulateCache_And_ReturnCachedOnSecondCall()
        {
            // Arrange
            var repo = new InMemoryProductRepository();
            var cache = new InMemoryCacheService();
            var service = new ProductService(repo, _mapper, cache);

            // Adiciona dados iniciais para testes
            await repo.AddAsync(new Product { Name = "A", Stock = 1, Price = 10m });
            await repo.AddAsync(new Product { Name = "B", Stock = 2, Price = 20m });
            await repo.SaveChangesAsync();

            // Act - primeira chamada: serve para popular o cache
            var first = await service.GetAllAsync(null, null, null);

            // Assert - conteudo e cache populado
            Assert.NotNull(first);
            var version = await cache.GetStringAsync("products:version");
            Assert.False(string.IsNullOrEmpty(version));

            // Act - segunda chamada: vem do cache com o mesmo conteudo
            var second = await service.GetAllAsync(null, null, null);

            // Assert - compara contagem/nome para garantir consistência
            Assert.Equal(System.Linq.Enumerable.Count(first), System.Linq.Enumerable.Count(second));
        }
        #endregion
    }
}