
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using tlcn_dotnet.AuthorizationAttributes;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Dto.CartDto;
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
            if (cartPaymentDto.ListCartDetailId == null || cartPaymentDto.ListCartDetailId.Count < 1)
                throw new GeneralException("NO ITEM IN CART", ApplicationConstant.BAD_REQUEST_CODE);
            return await _cartService.PayCurrentCart(authorization, cartPaymentDto);
        }

        [CustomAuthorize(Roles = "ROLE_ADMIN, ROLE_EMPLOYEE")]
        [HttpPut("processCart/{strId}")]
        public async Task<DataResponse> ProcessCart([FromHeader(Name = "Authorization")] string authorization,
            string strId, [FromBody] ProcessCartDto processCartDto)
        {
            long? id = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _cartService.ProcessCart(authorization, id.Value, processCartDto);
        }

        [CustomAuthorize]
        [HttpGet("cartHistory")]
        public async Task<DataResponse> GetCartHistory([FromHeader(Name = "Authorization")] string authorization,
            CartStatus? status, PaymentMethod? paymentMethod, string? fromDate, string? toDate,
            string? fromTotal, string? toTotal, string? sortBy = "PURCHASEDATE", string? order = "DESC", string? page = "1", string? pageSize = "5")
        {
            return await _cartService.GetCartHistory(authorization, status, paymentMethod,
                fromDate, toDate, fromTotal, toTotal, sortBy, order, page, pageSize);
        }

        [CustomAuthorize]
        [HttpGet("cartHistory/{strId}")]
        public async Task<DataResponse> GetCartHistoryById([FromHeader] string authorization,
            string strId)
        {
            long? id = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _cartService.GetCartHistoryById(authorization, id.Value);
        }

        [CustomAuthorize]
        [HttpGet("cartCancel/{strId}")]
        public async Task<DataResponse> CancelCart([FromHeader(Name = "Authorization")] string authorization, string strId)
        {
            long? id = Util.ParseId(strId) ??
                throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _cartService.CancelCart(authorization, id.Value);
        }

        [CustomAuthorize(Roles = "ROLE_ADMIN, ROLE_EMPLOYEE")]
        [HttpGet("processCart")]
        public async Task<DataResponse> ManageCart([FromQuery] RequestFilterProcessCartDto requestFilterProcessCart)
        {
            return await _cartService.ManageCart(requestFilterProcessCart);
        }

        [CustomAuthorize(Roles = "ROLE_ADMIN, ROLE_EMPLOYEE")]
        [HttpGet("processCart/{strId}")]
        public async Task<DataResponse> ProcessCartDetailById(string strId)
        {
            long? id = Util.ParseId(strId) ?? throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _cartService.ProcessCartDetailById(id.Value);
        }

        [CustomAuthorize]
        [HttpDelete("deleteCurrentCart")]
        public async Task<DataResponse> DeleteCurrentCart([FromHeader(Name = "Authorization")] string authorization)
        {
            return await _cartService.DeleteCurrentCart(authorization);
        }

        [CustomAuthorize]
        [HttpGet("validateQuantity")]
        public async Task<DataResponse> ValidateQuantity([FromHeader(Name = "Authorization")] string authorization)
        {
            return await _cartService.ValidateQuantity(authorization);
        }
    }
}
