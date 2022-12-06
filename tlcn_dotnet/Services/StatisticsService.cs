using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IBillRepository _billRepository;
        public StatisticsService(ICartRepository cartRepository, IBillRepository billRepository)
        {
            _cartRepository = cartRepository;
            _billRepository = billRepository;
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
