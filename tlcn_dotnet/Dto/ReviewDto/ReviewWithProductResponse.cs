using tlcn_dotnet.Dto.ProductDto;

namespace tlcn_dotnet.Dto.ReviewDto
{
    public class ReviewWithProductResponse: ReviewResponse
    {
        public ProductIdAndNameDto Product { get; set; }
    }
}
