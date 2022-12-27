using tlcn_dotnet.Dto.ProductImageDto;
using tlcn_dotnet.Entity;
using static System.Net.Mime.MediaTypeNames;

namespace tlcn_dotnet.Dto.ProductDto
{
    public class SingleImageProductDto: SimpleProductDto
    {
        public SimpleProductImageDto Image { get; set; }
    }
}
