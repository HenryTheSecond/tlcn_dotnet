using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Dto.ProductDto;
using tlcn_dotnet.Dto.SupplierDto;

namespace tlcn_dotnet.Dto.InventoryDto
{
    public class SimpleInventoryDto
    {
        public long? Id { get; set; }

        public ProductWithImageDto Product { get; set; }

        public double Quantity { get; set; }

        public Decimal ImportPrice { get; set; }

        public ProductUnit Unit { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public DateTime? ExpireDate { get; set; }

        public string? Description { get; set; }

        public SimpleSupplierDto? Supplier { get; set; }
    }
}
