
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.AuthorizationAttributes;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartDetailService _cartDetailService;
        private readonly ICartService _cartService;
        public CartController(ICartDetailService cartDetailService, ICartService cartService)
        {
            _cartDetailService = cartDetailService;
            _cartService = cartService;
        }

        [CustomAuthorize]
        [HttpPost]
        public async Task<DataResponse> AddToCart([FromHeader(Name = "Authorization")] string authorization, [FromBody] AddCartDetailRequest addCartDetailDto)
        {
            Console.WriteLine(addCartDetailDto.ProductId);
            return await _cartDetailService.AddCartDetail(authorization, addCartDetailDto);
        }

        [CustomAuthorize]
        [HttpDelete("{strId}")]
        public async Task<DataResponse> DeleteCartDetail([FromHeader(Name = "Authorization")] string authorization, string strId)
        {
            long? id = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _cartDetailService.DeleteCartDetail(authorization, id.Value);
        }

        [CustomAuthorize]
        [HttpPut]
        public async Task<DataResponse> UpdateQuantity([FromHeader(Name = "Authorization")] string authorization, [FromBody] UpdateCartDetailQuantityDto updateCartDetailQuantityDto)
        {
            return await _cartDetailService.UpdateCartDetailQuantity(authorization, updateCartDetailQuantityDto);
        }

        [CustomAuthorize]
        [HttpGet]
        public async Task<DataResponse> GetCurrentCart([FromHeader(Name = "Authorization")] string authorization)
        { 
            return await _cartDetailService.GetCurrentCart(authorization);
        }

        [CustomAuthorize]
        [HttpPost("payment")]
        public async Task<DataResponse> PayCurrentCart([FromHeader(Name = "Authorization")] string authorization, [FromBody] CartPaymentDto cartPaymentDto)
        {
            return await _cartService.PayCurrentCart(authorization, cartPaymentDto);
        }
    }
}
