namespace tlcn_dotnet.IServices
{
    public interface IStatisticsService
    {
        public Task<DataResponse> CountCartByStatus(string strFromDate, string strToDate);
    }
}
