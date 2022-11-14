using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.ProductDto
{
    public class AddProductDto
    {
        public long? CategoryId { get; set; }
        public string Name { get; set; }
        public Decimal Price { get; set; }
        public double? Quantity { get; set; }
        public ProductUnit Unit { get; set; }
        public double? MinPurchase { get; set; } = 0;
        public ProductStatus Status { get; set; }
        public string? Description { get; set; }
    }
}
