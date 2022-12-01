using AutoMapper;
using CloudinaryDotNet.Actions;
using System.IdentityModel.Tokens.Jwt;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto;
using tlcn_dotnet.Dto.BillDto;
using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Dto.CartDto;
using tlcn_dotnet.Dto.GhnItemDto;
using tlcn_dotnet.Dto.LocationDto;
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
        private readonly IDeliveryService _deliveryService;
        public CartService(ICartDetailRepository cartDetailRepository, IBillService billService, ICartRepository cartRepository, IMapper mapper, IDeliveryService deliveryService)
        {
            _cartDetailRepository = cartDetailRepository;
            _billService = billService;
            _cartRepository = cartRepository;
            _mapper = mapper;
            _deliveryService = deliveryService;
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

            IList<CartDetail> cartDetails = await _cartDetailRepository.GetListCart((long)accountId, cartPaymentDto.ListCartDetailId);

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
                Name = cartPaymentDto.Name,
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

        public async Task<DataResponse> ProcessCart(string authorization, long id, ProcessCartDto processCartDto)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            Cart cart = await _cartRepository.ProcessCart(id, accountId, processCartDto)
                ?? throw new GeneralException("CART NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            if (cart.Status == CartStatus.DELIVERIED)
            {
                HttpResponseMessage response = await _deliveryService.SendDeliveryRequest(await GhnParameters(cart));
                var a = (await response.Content.ReadFromJsonAsync<Dictionary<string, object>>())["code_message_value"];
                Console.WriteLine(a);
            }
            return new DataResponse(_mapper.Map<CartResponse>(cart));
        }

        private async Task<Dictionary<string, object>> GhnParameters(Cart cart)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("payment_type_id", 2);
            parameters.Add("required_note", "CHOXEMHANGKHONGTHU");
            parameters.Add("client_order_code", cart.Bill.Id.ToString());
            parameters.Add("to_name", cart.Name);
            parameters.Add("to_phone", cart.Phone);
            parameters.Add("to_address", cart.DetailLocation);
            VietnamLocationDto location = await Util.FindVietnamLocation(cart.CityId, cart.DistrictId, cart.WardId);
            parameters.Add("to_ward_name", location.Ward);
            parameters.Add("to_district_name", location.District);
            parameters.Add("to_province_name", location.City);
            parameters.Add("cod_amount", cart.Bill.PaymentMethod == PaymentMethod.CASH ? cart.Bill.Total : 0);
            parameters.Add("weight", 500);
            parameters.Add("length", 50);
            parameters.Add("width", 50);
            parameters.Add("height", 20);
            parameters.Add("service_id", 0);
            parameters.Add("service_type_id", 2);
            parameters.Add("items", CreateGhnItemList(cart.CartDetails));
            return parameters;
        }

        private IList<GhnItemDto> CreateGhnItemList(IList<CartDetail> cartDetails)
        { 
            IList<GhnItemDto> items = new List<GhnItemDto>();
            foreach(var cartDetail in cartDetails)
            {
                GhnItemDto item = new GhnItemDto()
                {
                    Name = cartDetail.Product.Name,
                    Code = cartDetail.Id.ToString(),
                    Quantity = cartDetail.Unit == ProductUnit.UNIT ? Convert.ToInt32(cartDetail.Quantity) : 1,
                    Price = Convert.ToInt32(cartDetail.Price),
                    Weight = cartDetail.Unit == ProductUnit.WEIGHT ? Convert.ToInt32((cartDetail.Quantity * 1000)) : 1
                };
                items.Add(item);
            }
            return items;
        }
    }
}
