using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Dto.ProductDto
{
    public class ProductAndPromotionDto
    {
        public Product Product { get; set; }
        public ProductPromotion Promotion { get; set; }
    }
}
