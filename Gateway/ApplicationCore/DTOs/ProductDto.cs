
namespace Gateway.ApplicationCore.DTOs
{
    public class ProductDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public ProductData? Data { get; set; }

        public class ProductData
        {
            public string? Color { get; set; }
            public string? Capacity { get; set; }
        }
    }
}
