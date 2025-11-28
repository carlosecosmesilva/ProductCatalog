using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            var redisConfig = configuration.GetSection("Redis")?["Configuration"];
            if (!string.IsNullOrEmpty(redisConfig))
            {
                services.AddStackExchangeRedisCache(opts =>
                {
                    opts.Configuration = redisConfig;
                });
            }

            services.AddScoped<ProductCatalog.Domain.Interfaces.IProductRepository, ProductCatalog.Infrastructure.Repositories.ProductRepository>();

            services.AddSingleton<ProductCatalog.Application.Interfaces.ICacheService, ProductCatalog.Infrastructure.Cache.RedisCacheService>();

            return services;
        }
    }
}
