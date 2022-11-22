using AutoMapper;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.BillDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;

namespace tlcn_dotnet.Services
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;
        private readonly IBillDetailRepository _billDetailRepository;
        private readonly IMapper _mapper;
        public BillService(IBillRepository billRepository, IBillDetailRepository billDetailRepository, IMapper mapper)
        {
            _billRepository = billRepository;
            _billDetailRepository = billDetailRepository;
            _mapper = mapper;
        }

        public async Task<DataResponse> CreateBill(IEnumerable<CartDetail> cartDetails, PaymentMethod paymentMethod)
        {
            decimal total = CalculateCart(cartDetails);
            long billId = await _billRepository.InsertBill(total, paymentMethod);
            List<BillDetail> billDetails = new List<BillDetail>();
            foreach (CartDetail cartDetail in cartDetails)
            {
                _billDetailRepository.InsertBillDetail(new BillDetail()
                {
                    BillId = billId,
                    ProductId = cartDetail.ProductId,
                    Unit = cartDetail.Unit,
                    Quantity = cartDetail.Quantity,
                    Price = cartDetail.Price.Value
                });
            }
            Bill bill = new Bill()
            {
                Total = total,
                Id = billId,
                PaymentMethod = paymentMethod
            };
            return new DataResponse(_mapper.Map<SimpleBillDto>(bill));
        }

        private decimal CalculateCart(IEnumerable<CartDetail> cartDetails)
        {
            decimal total = 0;
            foreach (CartDetail cartDetail in cartDetails)
            {
                cartDetail.Price = cartDetail.Product.Price;
                total += cartDetail.Price.Value * (decimal)cartDetail.Quantity;
            }
            return total;
        }
    }
}
