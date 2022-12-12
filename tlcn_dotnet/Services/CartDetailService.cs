using AutoMapper;
using Microsoft.OpenApi.Extensions;
using System.IdentityModel.Tokens.Jwt;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.BillDto;
using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class CartDetailService : ICartDetailService
    {
        private readonly ICartDetailRepository _cartDetailRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CartDetailService(ICartDetailRepository cartDetailRepository, IProductRepository productRepository, IMapper mapper)
        {
            _cartDetailRepository = cartDetailRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorization"></param>
        /// <param name="addCartDetailDto"></param>
        /// <returns></returns>
        /// <exception cref="GeneralException"></exception>
        public async Task<DataResponse> AddCartDetail(string authorization, AddCartDetailRequest addCartDetailDto)
        {
            JwtSecurityToken jwtToken = Util.ReadJwtToken(authorization);
            object accountId;
            jwtToken.Payload.TryGetValue("userId", out accountId);
            accountId = Convert.ToInt64(accountId);

            Product productDb = await _productRepository.GetById(addCartDetailDto.ProductId) ??
                throw new GeneralException("PRODUCT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);

            if (ValidateQuantityCartDetail(productDb, addCartDetailDto.Quantity) == false)
                throw new GeneralException("QUANTITY IS NOT VALID", ApplicationConstant.BAD_REQUEST_CODE);

            if (addCartDetailDto.Quantity > productDb.Quantity)
                throw new GeneralException("QUANTITY IS NOT ENOUGH", ApplicationConstant.BAD_REQUEST_CODE);

            long cartDetailExistId = await _cartDetailRepository.CheckCurrentCartHavingProduct((long)accountId, addCartDetailDto.ProductId);
            CartDetail cartDetail;

            if (cartDetailExistId == 0)
            {
                cartDetail = await _cartDetailRepository.AddCartDetail(new
                {
                    Status = CartDetailStatus.UNPAID.GetDisplayName(),
                    Unit = productDb.Unit.GetDisplayName(),
                    Quantity = addCartDetailDto.Quantity,
                    ProductId = addCartDetailDto.ProductId,
                    AccountId = accountId,
                });
            }
            else
            {
                cartDetail = await _cartDetailRepository.UpdateCartDetailQuantity(cartDetailExistId, addCartDetailDto.Quantity);
            }

            return new DataResponse(_mapper.Map<CartDetailResponse>(cartDetail));
        }

        public async Task<DataResponse> DeleteCartDetail(string authorization, long id)
        {
            JwtSecurityToken jwtToken = Util.ReadJwtToken(authorization);
            object accountId;
            jwtToken.Payload.TryGetValue("userId", out accountId);
            accountId = Convert.ToInt64(accountId);

            int affectedRows = await _cartDetailRepository.DeleteCartDetailByIdAndAccountId(id, (long)accountId);
            if (affectedRows == 0)
                throw new GeneralException("CANNOT DELETE CART ITEM", ApplicationConstant.NOT_FOUND_CODE);
            return new DataResponse(true);
        }

        public async Task<DataResponse> GetCurrentCart(string authorization)
        {
            JwtSecurityToken jwtToken = Util.ReadJwtToken(authorization);
            object accountId;
            jwtToken.Payload.TryGetValue("userId", out accountId);
            accountId = Convert.ToInt64(accountId);

            IList<CartDetail> cartDetails = await _cartDetailRepository.GetCurrentCart((long)accountId);

            return new DataResponse(_mapper.Map<IList<CartDetailResponse>>(cartDetails));
        }

        public async Task<DataResponse> UpdateCartDetailQuantity(string authorization, UpdateCartDetailQuantityDto updateCartDetailQuantityDto)
        {
            JwtSecurityToken jwtToken = Util.ReadJwtToken(authorization);
            object accountId;
            jwtToken.Payload.TryGetValue("userId", out accountId);
            accountId = Convert.ToInt64(accountId);

            Product product = await _productRepository.GetById(updateCartDetailQuantityDto.ProductId);
            if(ValidateQuantityCartDetail(product, updateCartDetailQuantityDto.Quantity) == false)
                throw new GeneralException("QUANTITY IS NOT VALID", ApplicationConstant.BAD_REQUEST_CODE);

            if (updateCartDetailQuantityDto.Quantity > product.Quantity)
                throw new GeneralException("QUANTITY IS NOT ENOUGH", ApplicationConstant.BAD_REQUEST_CODE);

            CartDetail cartDetail = await _cartDetailRepository.UpdateCartDetailQuantity(updateCartDetailQuantityDto.ProductId, updateCartDetailQuantityDto.Quantity, (long)accountId)
                ?? throw new GeneralException("CART DETAIL NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            return new DataResponse(_mapper.Map<CartDetailResponse>(cartDetail));
        }

        private bool ValidateQuantityCartDetail(Product product, double quantity)
        {
            if (product.Unit == ProductUnit.UNIT)
            {
                if (quantity - (int)quantity != 0)
                {
                    return false;
                }
            }
            if (product.MinPurchase == null || product.MinPurchase.Value == 0)
                return true;
            decimal decimalQuantity = Convert.ToDecimal(quantity);
            decimal decimalMinPurchase = Convert.ToDecimal(product.MinPurchase.Value);
            try
            {
                while (decimalMinPurchase != decimal.Ceiling(decimalMinPurchase))
                {
                    decimalMinPurchase *= 10;
                    decimalQuantity *= 10;
                }
            }
            catch (OverflowException e) //exception happens if min purchase is 0 or too small, almost equals to 0
            {
                return true;
            }
            if (decimalQuantity % decimalMinPurchase != 0)
                return false;
            return true;
        }
    }
}
