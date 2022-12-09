using AutoMapper;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.BillDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IBillRepository _billRepository;
        private readonly IMapper _mapper;
        public StatisticsService(ICartRepository cartRepository, IBillRepository billRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _billRepository = billRepository;
            _mapper = mapper;
        }

        public async Task<DataResponse> CalculateProfit(string strFromDate, string strToDate)
        {
            DateTime? fromDate = null, toDate = null;
            Util.TryConvertStringToDataType<DateTime>(strFromDate, out fromDate);
            Util.TryConvertStringToDataType<DateTime>(strToDate, out toDate);
            toDate = toDate == null ? null : toDate.Value.AddDays(1).AddTicks(-1);
            return new DataResponse(new { profit = await _billRepository.CalculateProfit(fromDate, toDate) });
        }

        public async Task<DataResponse> CountCartByStatus(string strFromDate, string strToDate)
        {
            DateTime? fromDate = null, toDate = null;
            Util.TryConvertStringToDataType<DateTime>(strFromDate, out fromDate);
            Util.TryConvertStringToDataType<DateTime>(strToDate, out toDate);
            toDate = toDate == null ? null : toDate.Value.AddDays(1).AddTicks(-1);
            return new DataResponse(await _cartRepository.CountCartByStatus(fromDate, toDate));
        }

        public async Task<DataResponse> StatisticBill(string? strFromDate, string? strToDate, string? strFromTotal,
            string? strToTotal, string? strPaymentMethod, string sortBy, string order)
        {

            sortBy = (sortBy.ToUpper() != "PURCHASEDATE" && sortBy.ToUpper() != "TOTAL") ? "PURCHASEDATE" : sortBy.ToUpper();
            order = (order.ToUpper() != "ASC" && order.ToUpper() != "DESC") ? "DESC" : order.ToUpper();
            DateTime ? fromDate = null, toDate = null;
            decimal? fromTotal = null, toTotal = null;
            PaymentMethod? paymentMethod = null;
            Util.TryConvertStringToDataType<DateTime>(strFromDate, out fromDate);
            Util.TryConvertStringToDataType<DateTime>(strToDate, out toDate);
            toDate = toDate == null ? null : toDate.Value.AddDays(1).AddTicks(-1);
            Util.TryConvertStringToDataType<decimal>(strFromTotal, out fromTotal);
            Util.TryConvertStringToDataType<decimal>(strToTotal, out toTotal);
            Util.TryConvertStringToDataType<PaymentMethod>(strPaymentMethod, out paymentMethod);
            IList<Bill> bills = await _billRepository.BillStatistic(fromDate, toDate, fromTotal, toTotal, paymentMethod, sortBy, order);
            return new DataResponse(_mapper.Map<IList<BillWithBillDetailDto>>(bills));
        }

        public async Task<DataResponse> StatisticProduct(string keyword, string strFromDate, string strToDate, string sortBy, string order)
        {
            if (sortBy.ToUpper() != "PRODUCTNAME" && sortBy.ToUpper() != "SALE" && sortBy.ToUpper() != "PROFIT")
                sortBy = "PRODUCTNAME";
            if (order.ToUpper() != "ASC" && order.ToUpper() != "DESC")
                order = "DESC";
            DateTime? fromDate = null, toDate = null;
            Util.TryConvertStringToDataType<DateTime>(strFromDate, out fromDate);
            Util.TryConvertStringToDataType<DateTime>(strToDate, out toDate);
            toDate = toDate == null ? null : toDate.Value.AddDays(1).AddTicks(-1);

            return new DataResponse(await _billRepository.ProductStatistic(keyword, fromDate, toDate, sortBy, order));
        }
    }
}
