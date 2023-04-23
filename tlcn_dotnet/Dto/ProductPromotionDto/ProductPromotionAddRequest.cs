using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.ProductPromotionDto
{
    public class ProductPromotionAddRequest
    {
        public long ProductId { get; set; }
        public ProductPromotionType Type { get; set; }
        public decimal Value { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
