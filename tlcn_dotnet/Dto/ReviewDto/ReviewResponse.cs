using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Dto.ReviewResponseDto;

namespace tlcn_dotnet.Dto.ReviewDto
{
    public class ReviewResponse
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public AccountReviewDto Account { get; set; }
        public double Rating { get; set; }
        public DateTime PostDate { get; set; }
        public long ProductId { get; set; }
        public IList<ReviewResourceResponse> ReviewResource { get; set; }
    }
}
