using tlcn_dotnet.Dto.ReviewDto;

namespace tlcn_dotnet.IServices
{
    public interface IReviewService
    {
        //public Task<DataResponse> GetAllProductReview(long id);
        public Task<DataResponse> ReviewProduct(string authorization, long productId, ReviewRequest reviewRequest);
    }
}
