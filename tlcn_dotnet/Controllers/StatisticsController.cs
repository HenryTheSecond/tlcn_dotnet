﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using tlcn_dotnet.AuthorizationAttributes;
using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.StatisticsDto;
using tlcn_dotnet.IServices;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }
        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        [HttpGet("countCart")]
        public Task<DataResponse> CountCart(string? fromDate, string? toDate)
        {
            return _statisticsService.CountCartByStatus(fromDate, toDate);
        }

        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        [HttpGet("profit")]
        public async Task<DataResponse> CalculateProfit(string? fromDate, string? toDate)
        {
            return await _statisticsService.CalculateProfit(fromDate, toDate);
        }

        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        [HttpGet("product")]
        public async Task<DataResponse> StatisicProduct(string? keyword, string? fromDate, string? toDate, string? sortBy = "PRODUCTNAME", string? order = "DESC")
        {
            return await _statisticsService.StatisticProduct(keyword, fromDate, toDate, sortBy, order);
        }

        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        [HttpGet("bill")]
        public async Task<DataResponse> StatisticProduct(string? fromDate, string? toDate, string? fromTotal,
            string? toTotal, string? paymentMethod, string? sortBy = "PURCHASEDATE", string? order = "DESC")
        {
            return await _statisticsService.StatisticBill(fromDate, toDate, fromTotal, toTotal, paymentMethod, sortBy, order);
        }

        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        [HttpGet("bill/export")]
        public async Task<FileResult> ExportStatisticBillToExcel(string? fromDate, string? toDate, string? fromTotal,
            string? toTotal, string? paymentMethod, string? sortBy = "PURCHASEDATE", string? order = "DESC")
        {
            MemoryStream stream = await _statisticsService.ExportStatisticBillToExcel(fromDate, toDate, fromTotal,
                toTotal, paymentMethod, sortBy, order);
            using (stream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString() + ".xlsx");
            }
        }

        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        [HttpGet("product/export")]
        public async Task<FileResult> ExportStatisticProductToExcel(string? keyword, string? fromDate, string? toDate, string? sortBy = "PRODUCTNAME", string? order = "DESC")
        {
            MemoryStream stream = await _statisticsService.ExportStatisticProductToExcel(keyword, fromDate, toDate, sortBy, order);
            using (stream)
            {
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString() + ".xlsx");
            }
        }

        [HttpGet("category")]
        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        public async Task<DataResponse> StatisticsByProductCategory(DateTime? from, DateTime? to)
        { 
            return await _statisticsService.StatisticsByProductCategory(from, to);
        }

        [HttpGet("countAllProduct")]
        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        public async Task<DataResponse> CountAllProduct()
        {
            return await _statisticsService.CountAllProduct();
        }

        [HttpGet("profitIn7Days")]
        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        public async Task<DataResponse> CalculateProfitIn7Days()
        {
            return await _statisticsService.CalculateProfitIn7Days();
        }

        [HttpGet("countAllUser")]
        [CustomAuthorize(Roles = "ROLE_ADMIN")]
        public async Task<DataResponse> CountAllUser()
        {
            return await _statisticsService.CountAllUser();
        }

        [HttpGet("top5User")]
        //[CustomAuthorize(Roles = "ROLE_ADMIN")]
        public async Task<DataResponse> Top5User(string? fromDate, string? toDate)
        {
            return await _statisticsService.Top5User(fromDate, toDate);
        }
    }
}
