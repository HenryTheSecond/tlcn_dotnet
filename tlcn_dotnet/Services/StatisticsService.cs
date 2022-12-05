using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ICartRepository _cartRepository;
        public StatisticsService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<DataResponse> CountCartByStatus(string strFromDate, string strToDate)
        {
            DateTime? fromDate = null, toDate = null;
            Util.TryConvertStringToDataType<DateTime>(strFromDate, out fromDate);
            Util.TryConvertStringToDataType<DateTime>(strToDate, out toDate);
            toDate = toDate == null ? null : toDate.Value.AddDays(1).AddTicks(-1);
            return new DataResponse(await _cartRepository.CountCartByStatus(fromDate, toDate));
        }
    }
}
