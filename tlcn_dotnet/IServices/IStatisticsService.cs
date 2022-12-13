using Microsoft.AspNetCore.Mvc;

namespace tlcn_dotnet.IServices
{
    public interface IStatisticsService
    {
        public Task<DataResponse> CountCartByStatus(string strFromDate, string strToDate);
        public Task<DataResponse> CalculateProfit(string strFromDate, string strToDate);
        public Task<DataResponse> StatisticProduct(string keyword, string strFromDate, string strToDate, string sortBy, string order);
        public Task<DataResponse> StatisticBill(string? strFromDate, string? strToDate, string? strFromTotal,
            string? strToTotal, string? strPaymentMethod, string sortBy, string order);
        public Task<MemoryStream> ExportStatisticBillToExcel(string? strFromDate, string? strToDate, string? strFromTotal,
            string? strToTotal, string? strPaymentMethod, string sortBy, string order);
        public Task<MemoryStream> ExportStatisticProductToExcel(string keyword, string strFromDate, string strToDate, string sortBy, string order);
    }
}
