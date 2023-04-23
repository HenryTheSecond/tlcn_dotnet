using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.ProductPromotionDto
{
    public class SimpleProductPromotionDto
    {
        public long Id { get; set; }
        public ProductPromotionType Type { get; set; }
        public decimal Value { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ExpireDate { get; set; }
        public bool IsEnable { get; set; } = true;
    }
}
