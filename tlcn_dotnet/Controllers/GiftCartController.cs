using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.AuthorizationAttributes;
using tlcn_dotnet.Dto.GiftCartDto;
using tlcn_dotnet.IServices;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [CustomAuthorize]
    public class GiftCartController : ControllerBase
    {
        private readonly IGiftCartService _giftCartService;
        public GiftCartController(IGiftCartService giftCartService)
        {
            _giftCartService = giftCartService;
        }

        [HttpPost("createGiftCart")]
        public async Task<DataResponse> CreateGiftCart([FromHeader(Name = "Authorization")] string authorization, [FromBody] GiftCartCreateRequest request)
        {
            return await _giftCartService.CreateGiftCart(authorization, request);
        }

        [HttpGet("getCurrentGiftCart")]
        public async Task<DataResponse> GetCurrentGiftCart([FromHeader(Name = "Authorization")] string authorization, [FromQuery] string? keyword)
        {
            return await _giftCartService.GetAllActiveGiftCart(authorization, keyword);
        }

        [HttpDelete("deleteGiftCart/{id}")]
        public async Task<DataResponse> DeleteGiftCart([FromHeader(Name = "Authorization")] string authorization, long id)
        {
            return await _giftCartService.DeleteGiftCart(authorization, id);
        }

        [HttpPut("changeGiftCartName/{id}")]
        public async Task<DataResponse> ChangeGiftCartName([FromHeader(Name = "Authorization")] string authorization, long id, [FromBody] GiftCartCreateRequest request)
        {
            return await _giftCartService.ChangeGiftCartName(authorization, id, request);
        }

        [HttpGet("product/{productId}")]
        [CustomAuthorize]
        public async Task<DataResponse> GetAllGiftCartWithProductId([FromHeader(Name = "Authorization")] string authorization, long productId)
        {
            return await _giftCartService.GetAllGiftCartWithProductId(authorization, productId);
        }
    }
}
