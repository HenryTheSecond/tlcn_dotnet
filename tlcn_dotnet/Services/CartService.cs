using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Nodes;
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
using tlcn_dotnet.Migrations;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class CartService : ICartService
    {
        private readonly ICartDetailRepository _cartDetailRepository;
        private readonly IBillService _billService;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly IDeliveryService _deliveryService;
        private readonly IBillRepository _billRepository;
        private readonly MyDbContext _dbContext;
        public CartService(ICartDetailRepository cartDetailRepository, IBillService billService,
            ICartRepository cartRepository, IMapper mapper, IDeliveryService deliveryService, IBillRepository billRepository)
        {
            _cartDetailRepository = cartDetailRepository;
            _billService = billService;
            _cartRepository = cartRepository;
            _mapper = mapper;
            _deliveryService = deliveryService;
            _billRepository = billRepository;
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
            if (cartDetails.Count < 1)
                throw new GeneralException("NO ITEM TO PAY", ApplicationConstant.BAD_REQUEST_CODE);

            foreach (CartDetail cartDetail in cartDetails)
            {
                if (cartDetail.Product.Status == ProductStatus.UNSOLD)
                    throw new GeneralException($"PRODUCT {cartDetail.Product.Name} IS UNSOLD");
                if (cartDetail.Quantity > cartDetail.Product.Quantity)
                    throw new GeneralException($"ITEM {cartDetail.Product.Name} IS NOT ENOUGH");
            }

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
                    _cartDetailRepository.UpdatePriceAndCartId(cartDetail.Id.Value,
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
                await _billRepository.UpdateProductQuantityAfterProcess(cart.Bill.Id.Value);
                HttpResponseMessage response = await _deliveryService.SendDeliveryRequest(await GhnParameters(cart, processCartDto));
                var data = (await response.Content.ReadFromJsonAsync<JsonNode>());
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(data));
                string orderCode = data["data"]["order_code"].ToString();
                await _billRepository.UpdateBillOrderCode(cart.Bill.Id.Value, orderCode);

                DateTime now = DateTime.Now;
                if (cart.Bill.PaymentMethod == PaymentMethod.CASH)
                {
                    await _billRepository.UpdateBillPurchaseDate(cart.Bill.Id.Value, now);
                    cart.Bill.PurchaseDate = now;
                }
                cart.Bill.OrderCode = orderCode;
            }
            else if (cart.Status == CartStatus.CANCELLED)
            {
                await _billRepository.UpdateBillPurchaseDate(cart.Bill.Id.Value, null);
                cart.Bill.PurchaseDate = null;

/*                await _billRepository.DeleteBillById(cart.Bill.Id.Value);
                cart.Bill = null;*/

                //TODO Refund payment
            }
            return new DataResponse(_mapper.Map<CartResponse>(cart));
        }

        private async Task<Dictionary<string, object>> GhnParameters(Cart cart, ProcessCartDto processCartDto)
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
            parameters.Add("cod_amount", cart.Bill.PaymentMethod == PaymentMethod.CASH ? Convert.ToInt32(cart.Bill.Total) : Convert.ToInt32(0));
            parameters.Add("weight", processCartDto.Weight);
            parameters.Add("length", processCartDto.Length);
            parameters.Add("width", processCartDto.Width);
            parameters.Add("height", processCartDto.Height);
            parameters.Add("service_id", 53320);
            parameters.Add("service_type_id", 2);
            parameters.Add("items", CreateGhnItemList(cart.CartDetails));
            Console.WriteLine(JsonConvert.SerializeObject(parameters));
            return parameters;
        }

        private IList<GhnItemDto> CreateGhnItemList(IList<CartDetail> cartDetails)
        {
            IList<GhnItemDto> items = new List<GhnItemDto>();
            foreach (var cartDetail in cartDetails)
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

        public async Task<DataResponse> GetCartHistory(string authorization, CartStatus? status, PaymentMethod? paymentMethod,
            string? strFromDate, string? strToDate, string? strFromTotal,
            string? strToTotal, string? sortBy, string? order, string? strPage, string? strPageSize)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            DateTime? fromDate = null, toDate = null;
            decimal? fromTotal = null, toTotal = null;
            int? page = 1;
            int? pageSize = 5;
            if (sortBy.ToUpper() != "PURCHASEDATE" && sortBy.ToUpper() != "TOTAL")
                sortBy = "PURCHASEDATE";
            if (order.ToUpper() != "DESC" && order.ToUpper() != "ASC")
                order = "DESC";
            try
            {
                fromDate = Util.ConvertStringToDataType<DateTime>(strFromDate);
                toDate = Util.ConvertStringToDataType<DateTime>(strToDate);
                toDate = toDate != null ? toDate.Value.AddDays(1).AddTicks(-1) : null;
                fromTotal = Util.ConvertStringToDataType<decimal>(strFromTotal);
                toTotal = Util.ConvertStringToDataType<decimal>(strToTotal);
                page = Util.TryConvertStringToDataType<int>(strPage, out page) ? (page > 0 ? page : 1) : 1;
                pageSize = Util.TryConvertStringToDataType<int>(strPageSize, out pageSize) ? (pageSize > 0 ? pageSize : 5) : 5;
            }
            catch (Exception e)
            {
                throw new GeneralException(ApplicationConstant.BAD_REQUEST, ApplicationConstant.BAD_REQUEST_CODE);
            }
            dynamic result = await _cartRepository.GetUserCartHistory(accountId, status, paymentMethod, fromDate, toDate,
                fromTotal, toTotal, sortBy, order, page.Value, pageSize.Value);
            return new DataResponse
                (
            new
            {
                carts = _mapper.Map<IList<CartResponse>>(result.carts),
                maxPage = Util.CalculateMaxPage(result.count, pageSize.Value),
                currentPage = page
            }
                );
        }

        public async Task<DataResponse> CancelCart(string authorization, long id)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            Cart cart = await _cartRepository.GetById(id, accountId)
                ?? throw new GeneralException("CART NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            if (cart.Status == CartStatus.DELIVERIED)
                throw new GeneralException("CART ALREADY DELIVERIED, CANNOT CANCELLED", ApplicationConstant.FAILED_CODE);
            else if (cart.Status == CartStatus.CANCELLED)
                throw new GeneralException("CART HAS BEEN CANCELLED", ApplicationConstant.FAILED_CODE);

            await _cartRepository.UpdateCartStatus(id, CartStatus.CANCELLED);

            //await _billRepository.DeleteBillById(cart.Bill.Id.Value);
            await _billRepository.UpdateBillPurchaseDate(cart.Bill.Id.Value, null);


            return new DataResponse()
            {
                Message = ApplicationConstant.SUCCESSFUL,
                Status = ApplicationConstant.SUCCESSFUL_CODE,
                Data = true,
                DetailMessage = "CANCEL CART SUCCESSFULLY"
            };
        }

        public async Task<DataResponse> ManageCart(RequestFilterProcessCartDto requestFilterProcessCartDto)
        {
            DateTime? fromCreatedDate = null, toCreatedDate = null;
            decimal? fromTotal = null, toTotal = null;
            int? page = 1, pageSize = 5;
            string? sortBy = requestFilterProcessCartDto.SortBy != null ? requestFilterProcessCartDto.SortBy.ToUpper() : "CREATEDDATE";
            sortBy = (sortBy != "CREATEDDATE" && sortBy != "TOTAL") ? "CREATEDDATE" : sortBy;
            string? order = requestFilterProcessCartDto.Order != null ? requestFilterProcessCartDto.Order.ToUpper() : "ASC";
            order = (order != "ASC" && order != "DESC") ? "ASC" : order;
            string? keywordType = requestFilterProcessCartDto.KeyWordType != null ? requestFilterProcessCartDto.KeyWordType.ToUpper() : null;
            keywordType = (keywordType != "NAME" && keywordType != "PHONE") ? null : keywordType;

            Util.TryConvertStringToDataType<DateTime>(requestFilterProcessCartDto.FromCreatedDate, out fromCreatedDate);
            Util.TryConvertStringToDataType<DateTime>(requestFilterProcessCartDto.ToCreatedDate, out toCreatedDate);
            toCreatedDate = toCreatedDate == null ? null : toCreatedDate.Value.AddDays(1).AddTicks(-1);
            Util.TryConvertStringToDataType<decimal>(requestFilterProcessCartDto.FromTotal, out fromTotal);
            Util.TryConvertStringToDataType<decimal>(requestFilterProcessCartDto.ToTotal, out toTotal);
            Util.TryConvertStringToDataType<int>(requestFilterProcessCartDto.Page, out page);
            page = page == null ? 1 : (page < 0 ? 1 : page);
            Util.TryConvertStringToDataType<int>(requestFilterProcessCartDto.PageSize, out pageSize);
            pageSize = pageSize == null ? 5 : (pageSize < 0 ? 5 : pageSize);

            var result = await _cartRepository.GetCarts(keywordType, requestFilterProcessCartDto.KeyWord,
                requestFilterProcessCartDto.CityId, requestFilterProcessCartDto.DistrictId,
                requestFilterProcessCartDto.WardId, fromCreatedDate, toCreatedDate,
                fromTotal, toTotal, requestFilterProcessCartDto.PaymentMethod, page.Value, pageSize.Value, status: requestFilterProcessCartDto.CartStatus,
                sortBy: sortBy, order: order);
            return new DataResponse(new
            {
                carts = _mapper.Map<IList<CartResponse>>(result.carts),
                maxPage = Util.CalculateMaxPage(result.count, pageSize.Value),
                currentPage = page.Value
            });
        }

        public async Task<DataResponse> DeleteCurrentCart(string authorization)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            int affectedRow = await _cartRepository.DeleteCurrentCart(accountId);
            if (affectedRow == 0)
                throw new GeneralException("NO ITEM IN CART", ApplicationConstant.NOT_FOUND_CODE);
            return new DataResponse(true);
        }

        public async Task<DataResponse> ProcessCartDetailById(long id)
        {
            return new DataResponse(_mapper.Map<CartResponse>(await _cartRepository.ProcessCartById(id)));
        }
    }
}
