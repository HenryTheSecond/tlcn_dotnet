using Dapper;
using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Dto.StatisticsDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class CategoryRepository :GenericRepository<Category>, ICategoryRepository
    {
        private readonly DapperContext _dapperContext;
        public CategoryRepository(MyDbContext dbContext, DapperContext dapperContext): base(dbContext) 
        {
            _dapperContext = dapperContext;
        }

        public async Task<IList<CategoryStatisticsDto>> StatisticsByCategory(DateTime? from, DateTime? to)
        {
            string query = @" SELECT c.Name, c.Id, 
		                                sum(CASE WHEN b.PurchaseDate IS NULL THEN 0 WHEN b.Total IS NULL THEN 0 ELSE b.Total END) Profit, 
		                                sum(CASE WHEN b.PurchaseDate IS NULL THEN 0 WHEN bd.Quantity IS NULL THEN 0 ELSE bd.Quantity END) as Sales
                                FROM Category c LEFT OUTER JOIN Product p ON c.Id = p.CategoryId
	                                LEFT OUTER JOIN BillDetail bd ON p.Id = bd.ProductId
	                                LEFT OUTER JOIN Bill b ON b.Id = bd.BillId ";
            List<string> conditions = new List<string>();
            if (from != null)
                conditions.Add(" b.PurchaseDate >= @From ");
            if (to != null)
                conditions.Add(" b.PurchaseDate <= @To ");
            string where = conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : string.Empty;
            using (var connection = _dapperContext.CreateConnection())
            {
                var statistics = await connection.QueryAsync<CategoryStatisticsDto>($" {query} {where} GROUP BY c.Name, c.Id ", new { From = from, To = to });
                return statistics.ToList();
            }
        }
    }
}
