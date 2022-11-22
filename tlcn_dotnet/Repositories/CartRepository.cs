using Dapper;
using Microsoft.OpenApi.Extensions;
using tlcn_dotnet.Constant;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly DapperContext _dapperContext;
        public CartRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }
        public async Task<long> InsertCart(Cart cart)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"Insert into Cart (BillId, Phone, CityId, DistrictId, WardId, 
                                                    DetailLocation, Status, CreatedDate)
                                OUTPUT inserted.Id
                                values (@BillId, @Phone, @CityId, @DistrictId, @WardId, 
                                        @DetailLocation, @Status, @CreatedDate)";
                DynamicParameters parameters = new DynamicParameters(cart);
                parameters.Add("Status", cart.Status.GetDisplayName());
                long id = await connection.ExecuteScalarAsync<long>(query, parameters);
                return id;
            }
        }
    }
}
