using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.ProductPromotionDto
{
    public class ProductPromotionUpdateRequest
    {
        public ProductPromotionType? Type { get; set; }
        public decimal? Value { get; set; }
        public DateTime? ExpireDate { get; set; }
        public bool? IsEnable { get; set; }
    }
}
