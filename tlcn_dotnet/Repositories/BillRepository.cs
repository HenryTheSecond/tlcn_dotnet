using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.OpenApi.Extensions;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Dto.InventoryDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly DapperContext _dapperContext;
        private readonly MyDbContext _dbContext;
        public BillRepository(DapperContext dapperContext, MyDbContext dbContext)
        {
            _dapperContext = dapperContext;
            _dbContext = dbContext;
        }

        public async Task<decimal> CalculateProfit(DateTime? fromDate, DateTime? toDate)
        {
            var query = _dbContext.Bill.Where(bill => bill.PurchaseDate != null);
            if (fromDate != null)
            {
                query = query.Where(bill => bill.PurchaseDate >= fromDate);
            }
            if (toDate != null)
            {
                query = query.Where(bill => bill.PurchaseDate <= toDate);
            }
            decimal profit = await query.SumAsync(bill => bill.Total) ?? 0;
            return profit;
        }

        public async Task<long> InsertBill(decimal total, PaymentMethod paymentMethod = PaymentMethod.CASH, DateTime? purchaseDate = null)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"Insert into Bill (PurchaseDate, Total, PaymentMethod)
                                OUTPUT inserted.Id
                                values (@PurchaseDate, @Total, @PaymentMethod)";
                DynamicParameters parameters = new DynamicParameters(new
                {
                    PurchaseDate = purchaseDate,
                    Total = total,
                    PaymentMethod = paymentMethod.GetDisplayName()
                });
                long id = await connection.ExecuteScalarAsync<long>(query, parameters);
                return id;
            }
        }

        public async Task<int> UpdateBillOrderCode(long id, string orderCode)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = "UPDATE Bill SET Bill.OrderCode = @OrderCode WHERE Bill.Id = @Id";
                int affectedRow = await connection.ExecuteAsync(query, new { Id = id, OrderCode = orderCode });
                return affectedRow;
            }
        }

        public async Task<Bill> UpdateBillPurchaseDate(long id, DateTime? date)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"UPDATE Bill 
                                SET Bill.PurchaseDate = @PurchaseDate 
                                OUTPUT inserted.*
                                WHERE Id = @Id";
                var bill = await connection.QuerySingleOrDefaultAsync<Bill>(query, new { PurchaseDate = date, Id = id });
                return bill;
            }
        }

        public async Task<Bill> UpdatePurchaseDate(long id, DateTime date)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"UPDATE Bill SET PurchaseDate = @PurchaseDate
                                    WHERE Id = @Id";
                int affectedRow = await connection.ExecuteAsync(query, new
                {
                    PurchaseDate = date,
                    Id = id
                });
                if (affectedRow == 0)
                    return null;
                query = @"SELECT * FROM Bill WHERE Id = @Id";
                Bill billDb = await connection.QuerySingleOrDefaultAsync<Bill>(query, new { Id = id });
                return billDb;
            }
        }

        public async Task<IList<dynamic>> ProductStatistic(string keyword, DateTime? fromDate, DateTime? toDate, string sortBy, string order)
        {
            string selectFrom = @" select Product.Id, Product.Name, Product.Unit,
		                                sum(CASE WHEN Bill.PurchaseDate IS NULL THEN 0 ELSE BillDetail.Quantity * BillDetail.Price END) as Profit,
		                                sum(CASE WHEN Bill.PurchaseDate IS NULL THEN 0 ELSE BillDetail.Quantity END) as Sale
                                from (Product left outer join BillDetail on Product.Id = BillDetail.ProductId)
	                                left outer join Bill on BillDetail.BillId = Bill.Id ";
            List<string> conditions = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            string where = "  ";
            string groupBy = " group by Product.Id, Product.Name, Product.Unit ";
            string orderBy = $" {order} ";

            if (sortBy == "PROFIT")
                orderBy = " Product.Name " + orderBy;
            else if (sortBy == "SALE")
                orderBy = " SALE " + orderBy;
            else
                orderBy = " Product.Name " + orderBy;
            orderBy = " ORDER BY " + orderBy;

            if (keyword != null)
            {
                conditions.Add(" Product.Name LIKE @Keyword");
                parameters.Add("Keyword", "%" + keyword + "%");
            }
            if (fromDate != null)
            {
                conditions.Add(" Bill.PurchaseDate >= @FromDate ");
                parameters.Add("FromDate", fromDate);
            }
            if (toDate != null)
            {
                conditions.Add(" Bill.PurchaseDate <= @ToDate ");
                parameters.Add("ToDate", toDate);
            }
            if(conditions.Count > 0)
                where += " WHERE " + string.Join(" AND ", conditions);
            string query = $"{selectFrom} {where} {groupBy} {orderBy}";
            using (var connection = _dapperContext.CreateConnection())
            {
                return (await connection.QueryAsync<dynamic>(query, parameters)).ToList();
            }
        }
    }
}
