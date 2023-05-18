using AutoMapper;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.GiftCartDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class GiftCartService : IGiftCartService
    {
        private readonly IGiftCartRepository _giftCartRepository;
        private readonly IMapper _mapper;
        public GiftCartService(IGiftCartRepository giftCartRepository, IMapper mapper)
        {
            _giftCartRepository = giftCartRepository;
            _mapper = mapper;
        }

        public async Task<DataResponse> ChangeGiftCartName(string authorization, long id, GiftCartCreateRequest request)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            GiftCart giftCart = await _giftCartRepository.FindByIdAndAccountId(id, accountId);
            if (giftCart == null)
                throw new GeneralException("GIFT CART IS NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            if (giftCart.IsActive == false)
                throw new GeneralException("CANNOT CHANGE THIS GIFT CART NAME", ApplicationConstant.BAD_REQUEST_CODE);
            var result = await _giftCartRepository.UpdateGiftCartName(id, request.Name);
            return new DataResponse(result > 0);
        }

        public async Task<DataResponse> CreateGiftCart(string authorization, GiftCartCreateRequest request)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            GiftCart giftCart = await _giftCartRepository.CreateGiftCart(accountId, request);
            return new DataResponse(_mapper.Map<GiftCartResponse>(giftCart));
        }

        public async Task<DataResponse> DeleteGiftCart(string authorization, long id)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            var giftCart = await _giftCartRepository.FindByIdAndAccountId(id, accountId);
            if (giftCart == null)
                throw new GeneralException("GIFT CART IS NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            if (giftCart.IsActive == false)
                throw new GeneralException("CANNOT REMOVE THIS GIFT CART", ApplicationConstant.BAD_REQUEST_CODE);
            var result = await _giftCartRepository.DeleteGiftCart(giftCart);
            return new DataResponse(result > 0);
        }

        public async Task<DataResponse> GetAllActiveGiftCart(string authorization, string keyword)
        {
            IList<GiftCart> giftCarts = await _giftCartRepository.GetAllActiveGiftCartByAccountId(Util.ReadJwtTokenAndGetAccountId(authorization), keyword);
            return new DataResponse(_mapper.Map<IList<GiftCartResponse>>(giftCarts));
        }
    }
}
