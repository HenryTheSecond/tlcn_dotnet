using tlcn_dotnet.Dto.ReviewDto;

namespace tlcn_dotnet.IServices
{
    public interface IReviewService
    {
        public Task<DataResponse> GetAllProductReview(long productId, int page = 1, int pageSize = 5);
        public Task<DataResponse> ReviewProduct(string authorization, long productId, ReviewRequest reviewRequest);
        public Task<DataResponse> EditReview(string authorization, long id, ReviewRequest reviewRequest);
        public Task<DataResponse> DeleteReview(string authorization, long id);
    }
}
