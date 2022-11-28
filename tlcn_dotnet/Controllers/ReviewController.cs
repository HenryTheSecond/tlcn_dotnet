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
        /*[HttpGet("product/{strId}")]
        public Task<DataResponse> GetAllProductReview(string strId)
        {
            long? id = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);


        }*/

        [HttpPost("product/{strId}")]
        [CustomAuthorize]
        public async Task<DataResponse> ReviewProduct([FromHeader(Name = "Authorization")] string authorization, string strId,
            [FromBody] ReviewRequest reviewRequest)
        {
            long? id = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _reviewService.ReviewProduct(authorization, id.Value, reviewRequest);
        }
    }
}
