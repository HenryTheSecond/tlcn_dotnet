namespace tlcn_dotnet.IServices
{
    public interface IStatisticsService
    {
        public Task<DataResponse> CountCartByStatus(string strFromDate, string strToDate);
        public Task<DataResponse> CalculateProfit(string strFromDate, string strToDate);
        public Task<DataResponse> StatisticProduct(string keyword, string strFromDate, string strToDate, string sortBy, string order);
    }
}
