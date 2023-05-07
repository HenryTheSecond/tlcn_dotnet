using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.AuthorizationAttributes;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.ReviewDto;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{strId}")]
        public async Task<DataResponse> GetAllProductReview(string strId, string? page = "1", string? pageSize = "5")
        {
            long? productId = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            int numberPage = 1;
            int numberPageSize = 5;
            if (int.TryParse(page, out numberPage) && int.TryParse(pageSize, out numberPageSize))
            {
                numberPage = numberPage < 1 ? 1 : numberPage;
                numberPageSize = numberPageSize < 1 ? 5 : numberPageSize;
            }
            else
            {
                throw new GeneralException(ApplicationConstant.BAD_REQUEST, ApplicationConstant.BAD_REQUEST_CODE);
            }
            return await _reviewService.GetAllProductReview(productId.Value, numberPage, numberPageSize);
        }

        /*        [HttpPost("product/{strId}")]
                [CustomAuthorize]
                public async Task<DataResponse> ReviewProduct([FromHeader(Name = "Authorization")] string authorization, string strId,
                    [FromBody] ReviewRequest reviewRequest)
                {
                    long? id = Util.ParseId(strId) ??
                        throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
                    return await _reviewService.ReviewProduct(authorization, id.Value, reviewRequest);
                }*/

        [HttpPost("product/{strId}")]
        [CustomAuthorize]
        public async Task<DataResponse> ReviewProduct([FromHeader(Name = "Authorization")] string authorization, string strId,
    [FromForm(Name = "video")] IFormFileCollection video, [FromForm(Name = "image")] IFormFileCollection image, 
    [FromForm(Name = "review")] string reviewRequest)
        {
            long? id = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _reviewService.ReviewProduct(authorization, id.Value, video, image, reviewRequest);
        }

        /*        [HttpPut("product/{strId}")]
                [CustomAuthorize]
                public async Task<DataResponse> EditReview([FromHeader(Name = "Authorization")] string authorization,
                    string strId, [FromBody] ReviewRequest reviewRequest)
                {
                    long? id = Util.ParseId(strId) ??
                        throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
                    return await _reviewService.EditReview(authorization, id.Value, reviewRequest);
                }*/

        [HttpPut("product/{strId}")]
        [CustomAuthorize]
        public async Task<DataResponse> EditReview([FromHeader(Name = "Authorization")] string authorization,
          string strId, [FromForm(Name = "video")] IFormFileCollection video, [FromForm(Name = "image")] IFormFileCollection image,
    [FromForm(Name = "review")] string reviewRequest)
        {
            long? id = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _reviewService.EditReview(authorization, id.Value, video, image, reviewRequest);
        }

        [HttpDelete("product/{strId}")]
        [CustomAuthorize]
        public async Task<DataResponse> DeleteReview([FromHeader(Name = "Authorization")] string authorization,
            string strId)
        {
            long? id = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _reviewService.DeleteReview(authorization, id.Value);
        }

        [HttpGet("product/{strId}/userReview")]
        [CustomAuthorize]
        public async Task<DataResponse> GetUserReviewOfProduct([FromHeader(Name = "Authorization")] string authorization, string strId)
        {
            long? id = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _reviewService.GetUserReviewOfProduct(authorization, id.Value);
        }

        [HttpDelete("admin/deleteReview/{strId}")]
        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        public async Task<DataResponse> AdminDeleteReview(string strId)
        {
            long? id = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _reviewService.AdminDeleteReview(id.Value);
        }

        [HttpGet("admin/searchReview")]
        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        public async Task<DataResponse> AdminSearchReview(string? strProductId, string? keyword = "", int? page = 1, int? pageSize = 5)
        {
            long? productId = null;
            if (strProductId != null)
                productId = Util.ParseId(strProductId) ??
                    throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _reviewService.SearchReview(keyword, productId, page.Value, pageSize.Value);
        }
    }
}
