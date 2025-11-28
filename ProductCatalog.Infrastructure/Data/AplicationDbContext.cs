using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.ToTable(t => t.HasCheckConstraint("CH_Product_Price", "Price >= 0"));
            });

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Teclado Mecânico", Stock = 50, Price = 299.99m },
                new Product { Id = 2, Name = "Mouse Sem Fio", Stock = 120, Price = 89.50m },
                new Product { Id = 3, Name = "Monitor Ultrawide", Stock = 15, Price = 1899.00m },
                new Product { Id = 4, Name = "Headset Gamer", Stock = 80, Price = 199.90m },
                new Product { Id = 5, Name = "Webcam HD", Stock = 35, Price = 120.00m }
            );
        }
    }
}