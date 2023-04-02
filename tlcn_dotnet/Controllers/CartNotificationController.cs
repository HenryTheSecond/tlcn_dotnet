using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.AuthorizationAttributes;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CartNotificationController : ControllerBase
    {
        private readonly ICartNotificationService _cartNotificationService;
        public CartNotificationController(ICartNotificationService cartNotificationService)
        {
            _cartNotificationService = cartNotificationService;
        }

        [CustomAuthorize]
        [HttpGet]
        public async Task<DataResponse> GetCartNotification([FromHeader(Name = "Authorization")] string authorization, int offset = 0, int size = 5)
        {
            return await _cartNotificationService.GetCartNotification(authorization, offset, size);
        }

        [CustomAuthorize]
        [HttpGet("countNewNotifications")]
        public async Task<DataResponse> CountNewNotification([FromHeader(Name = "Authorization")] string authorization)
        {
            return await _cartNotificationService.CountNewNotifications(authorization);
        }

        [CustomAuthorize]
        [HttpGet("resetNewNotification")]
        public async Task<DataResponse> ResetNewNotification([FromHeader(Name = "Authorization")] string authorization)
        {
            return await _cartNotificationService.ResetNewNotification(authorization);
        }

        [CustomAuthorize]
        [HttpPut("{strId}")]
        public async Task<DataResponse> ReadNotification([FromHeader(Name = "Authorization")] string authorization, string strId)
        {
            long? id = Util.ParseId(strId) ?? throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _cartNotificationService.ReadNotification(authorization, id.Value);
        }
    }
}
