using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IServices;

namespace tlcn_dotnet.Services
{
    public class ReviewResourceService : IReviewResourceService
    {
        private readonly MyDbContext _dbContext;
        private readonly Cloudinary _cloudinary;
        public ReviewResourceService(MyDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;

            var cloudinarySection = configuration.GetSection("Cloudinary");
            string cloudName = cloudinarySection.GetSection("CloudName").Value;
            string apiKey = cloudinarySection.GetSection("ApiKey").Value;
            string apiSecret = cloudinarySection.GetSection("ApiSecret").Value;
            CloudinaryDotNet.Account cloudinaryAccount = new CloudinaryDotNet.Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(cloudinaryAccount);
            _cloudinary.Api.Secure = true;
        }
        private async Task<ImageUploadResult> UploadToCloudinary(MemoryStream ms, string fileName, ReviewResourceType type)
        {
            ImageUploadParams uploadParams;
            if (type == ReviewResourceType.VIDEO)
            {
                uploadParams = new VideoUploadParams()
                {
                    File = new FileDescription(fileName, ms),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true
                };
            }
            else
            {
                uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, ms),
                    UseFilename = true,
                    UniqueFilename = false,
                    Overwrite = true
                };
            }
            return await _cloudinary.UploadAsync(uploadParams);
        }
        public async Task<ReviewResource> AddReviewResource(IFormFile file, long reviewId)
        {
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                string fileName = "review_" +  Guid.NewGuid().ToString();

                ImageUploadResult result = await UploadToCloudinary(ms, fileName, file.ContentType.Contains("video") ? ReviewResourceType.VIDEO : ReviewResourceType.IMAGE); ;
                ReviewResource reviewResource = new ReviewResource
                {
                    FileName = fileName,
                    ReviewId = reviewId,
                    Type = file.ContentType.Contains("video") ? Constant.ReviewResourceType.VIDEO : Constant.ReviewResourceType.IMAGE,
                    Url = result.Url.ToString()
                };
                _dbContext.ReviewResource.Add(reviewResource);
                await _dbContext.SaveChangesAsync();
                return reviewResource;
            }
        }

        public async Task<ReviewResource> UpdateReviewResource(IFormFile file, long reviewId)
        {
            ReviewResourceType type = file.ContentType.Contains("video") ? ReviewResourceType.VIDEO : ReviewResourceType.IMAGE;
            ReviewResource reviewResource = await _dbContext.ReviewResource
                .FirstOrDefaultAsync(reviewResource => reviewResource.ReviewId == reviewId && reviewResource.Type == type);
            if(reviewResource == null)
            {
                return await AddReviewResource(file, reviewId);
            }
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                ImageUploadResult result = await UploadToCloudinary(ms, reviewResource.FileName, reviewResource.Type);
                reviewResource.Url = result.Url.ToString();
                await _dbContext.SaveChangesAsync();
                return reviewResource;
            }
        }

        public async Task DeleteReviewResourceByReviewId(long reviewId, long accountId)
        {
            List<ReviewResource> reviewResources = await _dbContext.ReviewResource.Where(reviewResource => reviewResource.ReviewId == reviewId && reviewResource.Review.AccountId == accountId).ToListAsync();
            reviewResources.ForEach(async reviewResource =>
            {
                var deletionParams = new DeletionParams(reviewResource.FileName)
                {
                    ResourceType = reviewResource.Type == ReviewResourceType.VIDEO ? ResourceType.Video : ResourceType.Image
                };
                await _cloudinary.DestroyAsync(deletionParams);
            });
            _dbContext.ReviewResource.RemoveRange(reviewResources);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AdminDeleteReviewResourceByReviewId(long reviewId)
        {
            List<ReviewResource> reviewResources = await _dbContext.ReviewResource.Where(reviewResource => reviewResource.ReviewId == reviewId).ToListAsync();
            reviewResources.ForEach(async reviewResource =>
            {
                var deletionParams = new DeletionParams(reviewResource.FileName)
                {
                    ResourceType = reviewResource.Type == ReviewResourceType.VIDEO ? ResourceType.Video : ResourceType.Image
                };
                await _cloudinary.DestroyAsync(deletionParams);
            });
            _dbContext.ReviewResource.RemoveRange(reviewResources);
            await _dbContext.SaveChangesAsync();
        }
    }
}
