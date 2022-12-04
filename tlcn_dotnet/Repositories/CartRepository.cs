using Dapper;
using Microsoft.OpenApi.Extensions;
using System.Data;
using tlcn_dotnet.Constant;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Dto.CartDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using static System.Net.Mime.MediaTypeNames;

namespace tlcn_dotnet.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly DapperContext _dapperContext;
        public CartRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<dynamic> GetUserCartHistory(long accountId, string? status, string? paymentMethod, 
            DateTime? fromDate, DateTime? toDate, decimal? fromTotal, decimal? toTotal, 
            string? sortBy, string? order, int page, int pageSize)
        {
/*            SELECT*
FROM Cart JOIN CartDetail ON Cart.Id = CartDetail.CartId
                                    JOIN Bill ON Bill.Id = Cart.BillId
                                    JOIN Product ON CartDetail.ProductId = Product.Id
                                    JOIN Account ON Account.Id = CartDetail.AccountId
                                    OUTER APPLY(SELECT TOP 1 * FROM ProductImage WHERE ProductImage.ProductId = Product.Id) as img
                                WHERE Cart.Id IN(
                                    SELECT DISTINCT(Cart.Id)
                                    FROM Cart JOIN CartDetail ON Cart.Id = CartDetail.CartId

                                        JOIN Bill ON Bill.Id = Cart.BillId

                                        JOIN Product ON CartDetail.ProductId = Product.Id

                                        JOIN Account ON Account.Id = CartDetail.AccountId

                                        OUTER APPLY (SELECT TOP 1 * FROM ProductImage WHERE ProductImage.ProductId = Product.Id) as img

                                    WHERE Account.Id = 39

                                    ORDER BY Cart.Id
                                    OFFSET 0 ROWS FETCH NEXT 5 ROWS ONLY
								)
ORDER BY Cart.CreatedDate DESC*/

            using (var connection = _dapperContext.CreateConnection())
            {
                string from = @" FROM Cart JOIN CartDetail ON Cart.Id = CartDetail.CartId
                                    JOIN Bill ON Bill.Id = Cart.BillId
                                    JOIN Product ON CartDetail.ProductId = Product.Id
                                    JOIN Account ON Account.Id = CartDetail.AccountId
                                    OUTER APPLY (SELECT TOP 1 * FROM ProductImage WHERE ProductImage.ProductId = Product.Id) as img ";
                string where = " WHERE Account.Id = @AccountId ";
                string query = @" SELECT * " + from;
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("AccountId", accountId);
                if (status != null && Enum.TryParse<CartStatus>(status, out _))
                {
                    where += " AND Cart.Status = @Status ";
                    parameters.Add("Status", status.ToUpper().Trim());
                }
                if (paymentMethod != null && Enum.TryParse<PaymentMethod>(paymentMethod, out _))
                {
                    where += " AND Bill.PaymentMethod = @PaymentMethod ";
                    parameters.Add("PaymentMethod", paymentMethod.ToUpper().Trim());
                }
                if (fromDate != null)
                {
                    where += " AND Bill.PurchaseDate >= @FromDate ";
                    parameters.Add("FromDate", fromDate);
                }
                if (toDate != null)
                { 
                    where += " AND Bill.PurchaseDate <= @ToDate ";
                    parameters.Add("ToDate", toDate);
                }
                if (fromTotal != null)
                {
                    where += " AND Bill.Total >= @FromTotal ";
                    parameters.Add("FromTotal", fromTotal);
                }
                if (toTotal != null)
                {
                    where += " AND Bill.Total <= @ToTotal ";
                    parameters.Add("ToTotal", toTotal);
                }
                query += where;
                if (sortBy.ToUpper() == "PURCHASEDATE")
                {
                    query += " ORDER BY Cart.CreatedDate ";
                }
                else
                {
                    query += " ORDER BY Bill.Total ";
                }
                if (order.ToUpper() == "DESC")
                {
                    query += " DESC ";
                }
                else
                {
                    query += " ASC ";
                }

                /*query += " OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY ";
                parameters.Add("Skip", (page - 1) * pageSize);
                parameters.Add("Take", pageSize);*/
                Console.WriteLine(query);

                var cartDictionary = new Dictionary<long, Cart>();
                IList<Cart> carts = (await connection.QueryAsync<Cart, CartDetail, Bill, Product, Account, ProductImage, Cart>
                    (
                        query,
                        (cart, cartDetail, bill, product, account, productImage) =>
                        {
                            product.ProductImages.Add(productImage);
                            cartDetail.Account = account;
                            cartDetail.Product = product;
                            cartDetail.ProductId = product.Id;

                            Cart cartEntry;
                            if (cartDictionary.TryGetValue(cart.Id.Value, out cartEntry) == false)
                            {
                                cartEntry = cart;
                                cartEntry.CartDetails = new List<CartDetail>();
                                cartDictionary.Add(cartEntry.Id.Value, cartEntry);
                                cartEntry.Bill = bill;
                                cartEntry.BillId = bill.Id;
                            }
                            cartEntry.CartDetails.Add(cartDetail);
                            return cartEntry;
                        },
                        param: parameters
                    )).Distinct().Skip((page - 1) * pageSize).Take(pageSize).ToList();
                long count = await connection.ExecuteScalarAsync<long>("SELECT COUNT(DISTINCT(Cart.Id)) " + from + where, parameters);
                return new { carts, count};
            }
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
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("ProcessDescription", processCartDto.ProcessDescription);
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

