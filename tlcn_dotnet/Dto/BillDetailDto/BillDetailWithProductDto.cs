using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;
using tlcn_dotnet.Dto.ProductDto;

namespace tlcn_dotnet.Dto.BillDetailDto
{
    public class BillDetailWithProductDto
    {
        public long Id { get; set; }

        public ProductUnit Unit { get; set; }
        public double Quantity { get; set; }
        public Decimal Price { get; set; }
        public ProductIdAndNameDto Product { get; set; }
    }
}
