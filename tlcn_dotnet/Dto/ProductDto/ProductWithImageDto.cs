using tlcn_dotnet.Dto.ProductImageDto;

namespace tlcn_dotnet.Dto.ProductDto
{
    public class ProductWithImageDto: SimpleProductDto
    {
        public IEnumerable<SimpleProductImageDto> ProductImages { get; set; }
    }
}
