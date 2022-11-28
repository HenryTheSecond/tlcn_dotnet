using tlcn_dotnet.Dto.ReviewDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IReviewRepository
    {
        //public IEnumerable<Review> GetAllProductReview(long id);

        public Task<long> InsertReview(long accountId, long productId, ReviewRequest reviewRequest);
    }
}
