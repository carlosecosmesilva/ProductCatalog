namespace ProductCatalog.Application.DTOs
{
    public class ProductCreateUpdateDTO
    {
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal Price { get; set; }
    }
}
