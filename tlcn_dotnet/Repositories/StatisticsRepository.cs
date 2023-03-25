using Dapper;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Dto.StatisticsDto;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly DapperContext _dapperContext;
        public StatisticsRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IList<ProfitByAccount>> ProfitByAccount(DateTime? from, DateTime? to)
        {
            string conditions = string.Empty;
            DynamicParameters parameters = new DynamicParameters();
            if (from != null)
            {
                conditions += " AND Bill.PurchaseDate >= @from ";
                parameters.Add("from", from);
            }
            if (to != null)
            {
                conditions += " AND Bill.PurchaseDate <= @to ";
                parameters.Add("to", to);
            }
            string query = $@" SELECT Account.Id, Account.Phone, Account.Email, Account.FirstName, Account.LastName, Account.Role,
		                                Account.CityId, Account.DistrictId, Account.WardId, Account.DetailLocation, Account.VerifyToken, Account.PhotoUrl,
		                                SUM
		                                (
			                                CASE WHEN Bill.Total IS NULL THEN 0
			                                ELSE Bill.Total END
		                                ) as Profit
                                FROM Account OUTER APPLY (SELECT DISTINCT CartId FROM CartDetail WHERE CartDetail.AccountId = Account.Id) CartDetail
                                                      LEFT OUTER JOIN Cart ON CartDetail.CartId = Cart.Id AND Cart.Status = 'DELIVERIED'
                                                      OUTER APPLY (SELECT * FROM Bill WHERE Bill.PurchaseDate IS NOT NULL AND Cart.BillId = Bill.Id {conditions} ) Bill
                                GROUP BY Account.Id, Account.Phone, Account.Email, Account.FirstName, Account.LastName, Account.Role,
		                                Account.CityId, Account.DistrictId, Account.WardId, Account.DetailLocation, Account.VerifyToken, Account.PhotoUrl
                                ORDER BY Profit DESC";
            using (var connection = _dapperContext.CreateConnection())
            {
                var result = await connection.QueryAsync<AccountResponse, decimal, ProfitByAccount>(query, (accountResponse, profit) =>
                {
                    return new ProfitByAccount() { Account = accountResponse, Profit = profit };
                }, parameters, splitOn: "Id, Profit");
                return result.ToList();
            }
            
        }
    }
}
