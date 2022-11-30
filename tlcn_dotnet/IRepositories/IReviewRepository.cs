using tlcn_dotnet.Dto.ReviewDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IReviewRepository
    {
        public Task<IEnumerable<Review>> GetAllProductReview(long productId, int page = 1, int pageSize = 5);
        public Task<long> CountProductReview(long productId);
        public Task<long> InsertReview(long accountId, long productId, ReviewRequest reviewRequest);
    }
}
