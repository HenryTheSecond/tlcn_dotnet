using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Dto.CategoryDto;

namespace tlcn_dotnet.Dto.ProductDto
{
    public class SimpleProductDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public Decimal? Price { get; set; }
        public ProductUnit Unit { get; set; }
        public double? MinPurchase { get; set; } = 0;
        public ProductStatus Status { get; set; }
        public string? Description { get; set; }
        public SimpleCategoryDto? Category { get; set; }
        public double? Quantity { get; set; }
        
        public double? Rating { get; set; }
        public double? Sales { get; set; } = 0;

    }
}
