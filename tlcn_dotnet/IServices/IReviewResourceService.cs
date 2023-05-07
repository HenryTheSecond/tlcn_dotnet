using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IServices
{
    public interface IReviewResourceService
    {
        Task<ReviewResource> AddReviewResource(IFormFile file, long reviewId);
        Task<ReviewResource> UpdateReviewResource(IFormFile file, long reviewId);
        Task DeleteReviewResourceByReviewId(long reviewId, long accountId);
        Task AdminDeleteReviewResourceByReviewId(long reviewId);
    }
}
