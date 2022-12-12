using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Dto.ProductDto
{
    public class ProductWithIdNameUnitDto: ProductIdAndNameDto
    {
        public ProductUnit Unit { get; set; }
    }
}
