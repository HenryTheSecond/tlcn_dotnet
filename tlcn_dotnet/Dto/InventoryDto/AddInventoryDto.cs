using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.InventoryDto
{
    public class AddInventoryDto
    {
        public long? ProductId { get; set; }
        public double Quantity { get; set; }
        public decimal ImportPrice { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Description { get; set; }
        public long? SupplierId { get; set; }
        public ProductUnit Unit { get; set; }
    }
}
