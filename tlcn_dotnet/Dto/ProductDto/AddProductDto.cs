using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Dto.ProductDto
{
    [UnitValidator]
    public class AddProductDto
    {
        public long? CategoryId { get; set; }
        public string Name { get; set; }
        [Range(0, double.PositiveInfinity, ErrorMessage = "PRICE IS INVALID")]
        public Decimal Price { get; set; }
        public ProductUnit Unit { get; set; }
        [Range(0, double.PositiveInfinity, ErrorMessage = "MIN PURCHASE IS INVALID")]
        public double? MinPurchase { get; set; } = 0;
        public ProductStatus Status { get; set; }
        public string? Description { get; set; }
        public double? Quantity { get; set; } = 0;
        public int? Weight { get; set; } = 0;
    }
}
