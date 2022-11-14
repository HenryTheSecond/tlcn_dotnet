using tlcn_dotnet.Dto.ProductImageDto;

namespace tlcn_dotnet.Dto.ProductDto
{
    public class ProductWithImageDto: SimpleProductDto
    {
        public ICollection<SimpleProductImageDto> ProductImages { get; set; }
    }
}
