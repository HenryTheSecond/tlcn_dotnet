using Dapper;
using Microsoft.OpenApi.Extensions;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Dto.InventoryDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class BillDetailRepository : IBillDetailRepository
    {
        private readonly DapperContext _dapperContext;
        public BillDetailRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }
        public async Task<long> InsertBillDetail(BillDetail billDetail)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"Insert into BillDetail (BillId, ProductId, 
                                                    Unit, Quantity, Price)
                                OUTPUT inserted.Id
                                values (@BillId, @ProductId, @Unit, @Quantity, @Price)";
                DynamicParameters parameters = new DynamicParameters(billDetail);
                parameters.Add("Unit", billDetail.Unit.GetDisplayName());
                long id = await connection.ExecuteScalarAsync<long>(query, parameters);
                return id;
            }
        }

    }
}
