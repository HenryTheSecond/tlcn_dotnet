using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Dto.InventoryDto
{
    [UnitValidator]
    public class AddInventoryDto
    {
        public long? ProductId { get; set; }
        [Range(0, double.PositiveInfinity, ErrorMessage = "QUANTITY IS INVALID")]
        public double Quantity { get; set; }
        [Range(0, double.PositiveInfinity, ErrorMessage = "IMPORT PRICE IS INVALID")]
        public decimal ImportPrice { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Description { get; set; }
        public long? SupplierId { get; set; }
        [IsEnum(EnumType = typeof(ProductUnit), ErrorMessage = "UNIT IS INVALID")]
        public ProductUnit Unit { get; set; }
    }
}
