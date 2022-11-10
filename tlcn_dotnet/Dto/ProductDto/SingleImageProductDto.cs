using tlcn_dotnet.Dto.ProductImageDto;

namespace tlcn_dotnet.Dto.ProductDto
{
    public class SingleImageProductDto: SimpleProductDto
    {
        public SimpleProductImageDto Image { get; set; }
    }
}
