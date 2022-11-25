using Dapper;
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
        public BillRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
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
    }
}
