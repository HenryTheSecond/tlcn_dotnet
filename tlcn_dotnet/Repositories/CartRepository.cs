using Dapper;
using Microsoft.OpenApi.Extensions;
using System.Data;
using tlcn_dotnet.Constant;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Dto.CartDto;
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
                                                    DetailLocation, Status, CreatedDate, Name)
                                OUTPUT inserted.Id
                                values (@BillId, @Phone, @CityId, @DistrictId, @WardId, 
                                        @DetailLocation, @Status, @CreatedDate, @Name)";
                DynamicParameters parameters = new DynamicParameters(cart);
                parameters.Add("Status", cart.Status.GetDisplayName());
                long id = await connection.ExecuteScalarAsync<long>(query, parameters);
                return id;
            }
        }

        public async Task<Cart> ProcessCart(long id, long accountId, ProcessCartDto processCartDto)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                Dictionary<long, Cart> cartDictionary = new Dictionary<long, Cart>();
                string procedure = "sp_ProcessCart";
                DynamicParameters parameters = new DynamicParameters(processCartDto);
                parameters.Add("Status", processCartDto.Status.GetDisplayName());
                parameters.Add("Id", id);
                parameters.Add("ProcessAccountId", accountId);
                Cart cart = (await connection.QueryAsync<Cart, Bill, CartDetail, Product, Category, Cart>(procedure,
                    (cart, bill, cartDetail, product, category) =>
                    {
                        Cart cartEntry;
                        if (!cartDictionary.TryGetValue(cart.Id.Value, out cartEntry))
                        { 
                            cartEntry = cart;
                            cartEntry.CartDetails = new List<CartDetail>();
                            cartEntry.Bill = bill;
                            cartDictionary.Add(cartEntry.Id.Value, cartEntry);
                        }
                        product.Category = category;
                        cartDetail.Product = product;
                        cartEntry.CartDetails.Add(cartDetail);
                        return cartEntry;
                    }, parameters,
                    commandType: CommandType.StoredProcedure))
                    .Distinct().SingleOrDefault();
                return cart;
            }
        }
    }
}

