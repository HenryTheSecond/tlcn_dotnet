using AutoMapper;
using CloudinaryDotNet.Actions;
using System.IdentityModel.Tokens.Jwt;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto;
using tlcn_dotnet.Dto.BillDto;
using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class CartService: ICartService
    {
        private readonly ICartDetailRepository _cartDetailRepository;
        private readonly IBillService _billService;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        public CartService(ICartDetailRepository cartDetailRepository, IBillService billService, ICartRepository cartRepository, IMapper mapper)
        {
            _cartDetailRepository = cartDetailRepository;
            _billService = billService;
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<DataResponse> PayCurrentCart(string authorization, CartPaymentDto cartPaymentDto)
        {
            string checkAddress = await Util.CheckVietnameseAddress(cartPaymentDto.CityId,
                cartPaymentDto.DistrictId, cartPaymentDto.WardId);
            if (checkAddress != null)
                throw new GeneralException(checkAddress, ApplicationConstant.BAD_REQUEST_CODE);

            JwtSecurityToken jwtToken = Util.ReadJwtToken(authorization);
            object accountId;
            jwtToken.Payload.TryGetValue("userId", out accountId);
            accountId = Convert.ToInt64(accountId);

            IEnumerable<CartDetail> cartDetails = await _cartDetailRepository.GetListCart((long)accountId, cartPaymentDto.ListCartDetailId);

            SimpleBillDto simpleBillDto = null;
            string paymentUrl = null;
            var createBillResponseData = (await _billService.CreateBill(cartDetails, cartPaymentDto.PaymentMethod)).Data;
            if (createBillResponseData.GetType() == typeof(SimpleBillDto))
                simpleBillDto = (SimpleBillDto)createBillResponseData;
            else 
            {
                simpleBillDto = ((PayingMomoBill)createBillResponseData).Bill;
                paymentUrl = ((PayingMomoBill)createBillResponseData).MomoPaymentLink;
            }
            Cart cart = new Cart()
            {
                BillId = simpleBillDto.Id,
                Phone = cartPaymentDto.Phone,
                CityId = cartPaymentDto.CityId,
                DistrictId = cartPaymentDto.DistrictId,
                WardId = cartPaymentDto.WardId,
                DetailLocation = cartPaymentDto.DetailLocation,
                Status = CartStatus.PENDING,
                CreatedDate = DateTime.Now,
            };

            long id = await _cartRepository.InsertCart(cart);
            List<Task<int>> insertPriceAndCartIdTasks = new List<Task<int>>();
            foreach (CartDetail cartDetail in cartDetails)
            {
                insertPriceAndCartIdTasks.Add(
                    _cartDetailRepository.InsertPriceAndCartId(cartDetail.Id.Value, 
                                                    cartDetail.Product.Price.Value, id));
            }
            await Task.WhenAll(insertPriceAndCartIdTasks);
            cart.Id = id;
            cart.CartDetails = cartDetails;

            CartResponse cartResponse = _mapper.Map<CartResponse>(cart);
            cartResponse.Bill = simpleBillDto;
            cartResponse.PaymentUrl = paymentUrl;

            return new DataResponse(cartResponse);
        }
    }
}
