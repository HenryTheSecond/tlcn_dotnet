using AutoMapper;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.BillDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;
        private readonly IBillDetailRepository _billDetailRepository;
        private readonly IMapper _mapper;
        private readonly IPaymentService _momoPaymentService;
        private readonly IProductPromotionRepository _productPromotionRepository;
        public BillService(IBillRepository billRepository, IBillDetailRepository billDetailRepository, IMapper mapper, IPaymentService momoPaymentService, IProductPromotionRepository productPromotionRepository)
        {
            _billRepository = billRepository;
            _billDetailRepository = billDetailRepository;
            _mapper = mapper;
            _momoPaymentService = momoPaymentService;
            _productPromotionRepository = productPromotionRepository;
        }

        public async Task<DataResponse> BillPaying(long id)
        {
            Bill bill = await _billRepository.UpdatePurchaseDate(id, DateTime.Now)
                ?? throw new GeneralException("BILL NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            return new DataResponse(_mapper.Map<SimpleBillDto>(bill));
        }

        public async Task<DataResponse> CreateBill(IEnumerable<CartDetail> cartDetails, PaymentMethod paymentMethod)
        {
            decimal total = CalculateCart(cartDetails);
            long billId = await _billRepository.InsertBill(total, paymentMethod);
            List<BillDetail> billDetails = new List<BillDetail>();
            List<Task<long>> insertBillDetailTasks = new List<Task<long>>();
            foreach (CartDetail cartDetail in cartDetails)
            {
                insertBillDetailTasks.Add
                    (
                        _billDetailRepository.InsertBillDetail(new BillDetail()
                        {
                            BillId = billId,
                            ProductId = cartDetail.ProductId,
                            Unit = cartDetail.Unit,
                            Quantity = cartDetail.Quantity,
                            Price = cartDetail.Price.Value
                        })
                    );
            }
            await Task.WhenAll(insertBillDetailTasks);
            Bill bill = new Bill()
            {
                Total = total,
                Id = billId,
                PaymentMethod = paymentMethod
            };

            if (paymentMethod == PaymentMethod.MOMO)
            {
                DateTime now = DateTime.Now;
                Bill billDb = await _billRepository.UpdatePurchaseDate(billId, now);
                Dictionary<string, object> momoRequestParameters = new Dictionary<string, object>();
                momoRequestParameters.Add("orderId", billDb.Id.ToString() + "_" + now.Ticks.ToString());
                momoRequestParameters.Add("amount", billDb.Total.Value);
                momoRequestParameters.Add("orderInfo", "Thanks for buying at One Winged Angel Fruit Shop");
                momoRequestParameters.Add("requestId", billDb.Id.ToString() + "_" + now.Ticks.ToString());
                HttpResponseMessage response = await _momoPaymentService.SendPaymentRequest(momoRequestParameters);
                SimpleBillDto payingMomoBill = _mapper.Map<SimpleBillDto>(billDb);
                return new DataResponse(new PayingMomoBill()
                { 
                    Bill = payingMomoBill,
                    MomoPaymentLink = (await response.Content.ReadFromJsonAsync<Dictionary<string, object>>())["payUrl"].ToString()
                });
            }

            return new DataResponse(_mapper.Map<SimpleBillDto>(bill));
        }

        public async Task<DataResponse> PayCod(long id)
        {
            return new DataResponse(await _billRepository.UpdateBillPurchaseDate(id, DateTime.Now));
        }

        private decimal CalculateCart(IEnumerable<CartDetail> cartDetails)
        {
            decimal total = 0;
            foreach (CartDetail cartDetail in cartDetails)
            {
                var promotion = _productPromotionRepository.GetPromotionByProductId(cartDetail.Product.Id.Value).Result;
                cartDetail.Price = Util.CalculatePrice(cartDetail.Product.Price.Value, promotion);
                total += cartDetail.Price.Value * (decimal)cartDetail.Quantity;
            }
            return total;
        }
    }
}
