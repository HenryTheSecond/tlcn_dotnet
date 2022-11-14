using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.ProductImageDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Dto.ProductDto
{
    public class EditProductDto
    {
        [Required]
        public string Name { get; set; }

        [Range(1, Double.MaxValue)]
        public Decimal? Price { get; set; }

        [Range(0, Double.MaxValue)]
        public double? Quantity { get; set; }
        public ProductUnit Unit { get; set; }

        [Range(0, Double.MaxValue)]
        public double? MinPurchase { get; set; } = 0;
        public ProductStatus Status { get; set; }
        public string? Description { get; set; }
        public long? CategoryId { get; set; }
        public IList<ProductImageEditStatus> EditImageStatus { get; set; }
    }
}
