namespace ProductCatalog.Domain.Entities
{

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Price { get; set; }

        public Product(string name, int stock, decimal price)
        {
            Name = name;
            Stock = stock;
            Price = price;
        }

        public Product() { }

        public void ValidatePrice()
        {
            if (Price < 0)
            {
                throw new ArgumentException("O valor do produto não pode ser negativo."); // Requisito: O valor do produto não pode ser negativo
            }
        }
    }
}


