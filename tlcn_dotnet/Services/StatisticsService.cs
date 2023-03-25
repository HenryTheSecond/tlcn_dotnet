using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using System.Collections.Immutable;
using System.Data;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Dto.BillDto;
using tlcn_dotnet.Dto.StatisticsDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly MyDbContext _dbContext;
        private readonly ICartRepository _cartRepository;
        private readonly IBillRepository _billRepository;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStatisticsRepository _statisticsRepository;
        public StatisticsService(MyDbContext dbContext, ICartRepository cartRepository, IBillRepository billRepository, ICategoryRepository categoryRepository, IMapper mapper, IStatisticsRepository statisticsRepository)
        {
            _dbContext = dbContext;
            _cartRepository = cartRepository;
            _billRepository = billRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _statisticsRepository = statisticsRepository;
        }

        public async Task<DataResponse> CalculateProfit(string strFromDate, string strToDate)
        {
            DateTime? fromDate = null, toDate = null;
            Util.TryConvertStringToDataType<DateTime>(strFromDate, out fromDate);
            Util.TryConvertStringToDataType<DateTime>(strToDate, out toDate);
            toDate = toDate == null ? null : toDate.Value.AddDays(1).AddTicks(-1);
            return new DataResponse(new { profit = await _billRepository.CalculateProfit(fromDate, toDate) });
        }

        public async Task<DataResponse> CalculateProfitIn7Days()
        {
            DateTime fromDate = DateTime.Now.AddDays(-7).Date;
            var profitIn7Days = _dbContext.Bill
                .Where(bill => bill.PurchaseDate > fromDate)
                .GroupBy(bill => bill.PurchaseDate.Value.Date)
                .Select(group => new ProfitByDayDto
                {
                    Date = group.Key.Date,
                    Profit = group.Sum(bill => bill.Total == null ? 0 : bill.Total.Value)
                }).ToDictionary(_ => _.Date, _ => _);

            for (int i = 0; i < 7; i++)
            {
                fromDate = fromDate.AddDays(1);
                if (!profitIn7Days.ContainsKey(fromDate.Date))
                    profitIn7Days.Add(fromDate.Date, new ProfitByDayDto { Date = fromDate.Date, Profit = 0 });
            }
            return new DataResponse(profitIn7Days.Values.OrderByDescending(_ => _.Date).ToList());
        }

        public async Task<DataResponse> CountAllProduct()
        {
            int count = await _dbContext.Product.Where(product => product.IsDeleted == false).CountAsync();
            return new DataResponse(count);
        }

        public async Task<DataResponse> CountAllUser()
        {
            int count = await _dbContext.Account.Where(account => account.Status == UserStatus.ACTIVE).CountAsync();
            return new DataResponse(count);
        }

        public async Task<DataResponse> CountCartByStatus(string strFromDate, string strToDate)
        {
            DateTime? fromDate = null, toDate = null;
            Util.TryConvertStringToDataType<DateTime>(strFromDate, out fromDate);
            Util.TryConvertStringToDataType<DateTime>(strToDate, out toDate);
            toDate = toDate == null ? null : toDate.Value.AddDays(1).AddTicks(-1);
            return new DataResponse(await _cartRepository.CountCartByStatus(fromDate, toDate));
        }

        public async Task<MemoryStream> ExportStatisticBillToExcel(string? strFromDate, string? strToDate, string? strFromTotal, string? strToTotal, string? strPaymentMethod, string sortBy, string order)
        {
            sortBy = (sortBy.ToUpper() != "PURCHASEDATE" && sortBy.ToUpper() != "TOTAL") ? "PURCHASEDATE" : sortBy.ToUpper();
            order = (order.ToUpper() != "ASC" && order.ToUpper() != "DESC") ? "DESC" : order.ToUpper();
            DateTime? fromDate = null, toDate = null;
            decimal? fromTotal = null, toTotal = null;
            PaymentMethod? paymentMethod = null;
            Util.TryConvertStringToDataType<DateTime>(strFromDate, out fromDate);
            Util.TryConvertStringToDataType<DateTime>(strToDate, out toDate);
            toDate = toDate == null ? null : toDate.Value.AddDays(1).AddTicks(-1);
            Util.TryConvertStringToDataType<decimal>(strFromTotal, out fromTotal);
            Util.TryConvertStringToDataType<decimal>(strToTotal, out toTotal);
            Util.TryConvertStringToDataType<PaymentMethod>(strPaymentMethod, out paymentMethod);
            IList<Bill> bills = await _billRepository.BillStatistic(fromDate, toDate, fromTotal, toTotal, paymentMethod, sortBy, order);
            DataTable dataTable = new DataTable($"Bill Statistics");
            dataTable.Columns.AddRange(new DataColumn[4]
            {
                new DataColumn("Id"),
                new DataColumn("Purchase Date"),
                new DataColumn("Total"),
                new DataColumn("Payment Method"),
            });
            foreach (Bill bill in bills)
            {
                dataTable.Rows.Add(bill.Id, bill.PurchaseDate, bill.Total, bill.PaymentMethod.GetDisplayName());
            }
            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dataTable);
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream;

            }
        }

        public async Task<MemoryStream> ExportStatisticProductToExcel(string keyword, string strFromDate, string strToDate, string sortBy, string order)
        {
            if (sortBy.ToUpper() != "PRODUCTNAME" && sortBy.ToUpper() != "SALE" && sortBy.ToUpper() != "PROFIT")
                sortBy = "PRODUCTNAME";
            if (order.ToUpper() != "ASC" && order.ToUpper() != "DESC")
                order = "DESC";
            DateTime? fromDate = null, toDate = null;
            Util.TryConvertStringToDataType<DateTime>(strFromDate, out fromDate);
            Util.TryConvertStringToDataType<DateTime>(strToDate, out toDate);
            toDate = toDate == null ? null : toDate.Value.AddDays(1).AddTicks(-1);

            IList<dynamic> productStatistics = await _billRepository.ProductStatistic(keyword, fromDate, toDate, sortBy, order);

            DataTable dataTable = new DataTable($"Product Statistics");
            dataTable.Columns.AddRange(new DataColumn[5]
            {
                new DataColumn("Id"),
                new DataColumn("Name"),
                new DataColumn("Unit"),
                new DataColumn("Profit"),
                new DataColumn("Sales")
            });
            foreach (var product in productStatistics)
            {
                dataTable.Rows.Add(product.Id, product.Name, product.Unit, product.Profit, product.Sale);
            }
            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(dataTable);
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream;
            }
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

        public async Task<DataResponse> StatisticsByProductCategory(DateTime? from, DateTime? to)
        {
            /*var query = _dbContext.BillDetail.Include(billDetail => billDetail.Bill)
                .Include(billDetail => billDetail.Product)
                .ThenInclude(product => product.Category);
            if (from != null)
                query.Where(billDetail => billDetail.Bill.PurchaseDate >= from);
            if (to != null)
                query.Where(billDetail => billDetail.Bill.PurchaseDate <= to);
            var groups = query.GroupBy(billDetail => new { billDetail.Product.Category.Id, billDetail.Product.Category.Name });
            var statistics = groups.Select(group => new CategoryStatisticsDto
            {
                Id = group.Key.Id.Value,
                Name = group.Key.Name,
                Profit = group.Sum(_ => _.Bill.Total ?? 0),
                Sales = group.Sum(_ => _.Quantity)
            }).ToList();
            return new DataResponse(statistics);*/

            return new DataResponse(await _categoryRepository.StatisticsByCategory(from, to));
        }

        public async Task<DataResponse> Top5User(string strFromDate, string strToDate)
        {
            DateTime? from = null, to = null;
            if (strFromDate != null && Util.TryConvertStringToDataType<DateTime>(strFromDate, out from) == false)
                throw new GeneralException("FROM DATE IS INVALID");
            if (strToDate != null && Util.TryConvertStringToDataType<DateTime>(strToDate, out to) == false)
                throw new GeneralException("TO DATE IS INVALID");
            to = to.Value.AddDays(1).AddTicks(-1);
            return new DataResponse(await _statisticsRepository.ProfitByAccount(from, to));
        }
    }
}
