using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Dto.ProductDto
{
    public class EditProductDto
    {
        public string Name { get; set; }
        public Decimal? Price { get; set; }
        public double? Quantity { get; set; }
        public ProductUnit Unit { get; set; }
        public double? MinPurchase { get; set; } = 0;
        public string? Description { get; set; }
        public long? CategoryId { get; set; }
    }
}
