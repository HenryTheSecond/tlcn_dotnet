using tlcn_dotnet.Constant;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Dto.ProductDto
{
    [UnitValidator]
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
