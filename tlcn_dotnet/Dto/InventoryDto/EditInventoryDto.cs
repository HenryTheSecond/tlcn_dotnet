using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Dto.InventoryDto
{
    [UnitValidator]
    public class EditInventoryDto
    {
        public long? ProductId { get; set; }
        public double? Quantity { get; set; }
        public decimal? ImportPrice { get; set; }
        [IsEnum(EnumType = typeof(ProductUnit), ErrorMessage = "UNIT IS INVALID")]
        public ProductUnit Unit { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string Description { get; set; }
        public long? SupplierId { get; set; }
    }
}
