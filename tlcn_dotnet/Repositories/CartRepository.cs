using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using MailKit.Search;
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

        public async Task<Dictionary<string, long>> CountCartByStatus(DateTime? fromDate, DateTime? toDate)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string where = string.Empty;
                DynamicParameters parameters = new DynamicParameters();
                bool ignoreFirstAndFlag = false;
                if (fromDate != null)
                {
                    where += " Cart.CreatedDate >= @FromDate ";
                    parameters.Add("FromDate", fromDate);
                    ignoreFirstAndFlag = true;
                }
                if (toDate != null)
                {
                    if (ignoreFirstAndFlag)
                        where += " AND ";
                    else
                        ignoreFirstAndFlag = false;
                    where += " Cart.CreatedDate <= @ToDate";
                    parameters.Add("ToDate", toDate);
                }
                if (where != string.Empty)
                    where = " WHERE " + where;
                string query = $"select Cart.Status, count(*) as count from Cart {where} group by Cart.Status";
                var counts = await connection.QueryAsync(query, parameters);
                Dictionary<string, long> result = new Dictionary<string, long>();
                foreach (string status in Enum.GetNames(typeof(CartStatus)))
                {
                    result.Add(status, 0);
                }
                foreach (var row in counts)
                {
                    result[row.Status] = row.count;
                }
                return result;
            }
        }

        public async Task<int> DeleteCurrentCart(long accountId)
        {
            string query = @"DELETE CartDetail WHERE CartDetail.AccountId = @AccountId AND CartDetail.CartId IS NULL";
            DynamicParameters parameters = new DynamicParameters(new { AccountId = accountId });
            using (var connection = _dapperContext.CreateConnection())
            {
                int affectedRows = await connection.ExecuteAsync(query, parameters);
                return affectedRows;
            }
        }

        public async Task<Cart> GetById(long id, long accountId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"SELECT * FROM Cart JOIN Bill ON Cart.BillId = Bill.Id
                                WHERE Cart.Id = @Id AND 
	                                Cart.Id IN 
		                                (SELECT TOP 1 CartDetail.CartId FROM CartDetail WHERE CartDetail.CartId = @Id AND CartDetail.AccountId = @AccountId)";
                Cart cart = (await connection.QueryAsync<Cart, Bill, Cart>(query,
                    (cart, bill) =>
                    {
                        cart.Bill = bill;
                        return cart;
                    },
                    new { Id = id, AccountId = accountId })).SingleOrDefault();
                Console.WriteLine($"CHECK {cart.Bill}");
                return cart;
            }
        }

        /*public async Task<dynamic> GetCarts(string? keywordType, string? keyword,
            string? cityId, string? districtId, string? wardId,
            DateTime? fromCreatedDate, DateTime? toCreatedDate, decimal? fromTotal, decimal? toTotal,
            PaymentMethod? paymentMethod, int page, int pageSize,
            CartStatus? status = CartStatus.PENDING,
            string? sortBy = "CREATEDDATE", string? order = "ASC")
        {
            string from = @" FROM Cart JOIN CartDetail ON Cart.Id = CartDetail.CartId
                                    JOIN Bill ON Bill.Id = Cart.BillId
                                    JOIN Product ON CartDetail.ProductId = Product.Id
                                    JOIN Account ON Account.Id = CartDetail.AccountId
                                    OUTER APPLY(SELECT TOP 1 * FROM ProductImage WHERE ProductImage.ProductId = Product.Id) as img ";
            string where = @"  ";
            List<string> conditions = new List<string>();
            string orderBy = " ";
            if (sortBy == "CREATEDDATE")
                orderBy += " ORDER BY Cart.CreatedDate ";
            else
                orderBy += " ORDER BY Bill.Total ";
            orderBy += order;
            DynamicParameters parameters = new DynamicParameters();
            if (keywordType != null)
            {
                if (keywordType == "NAME")
                {
                    conditions.Add(" Cart.Name LIKE @Keyword ");
                }
                else
                {
                    conditions.Add(" Cart.Phone LIKE @Keyword ");
                }
                parameters.Add("Keyword", "%" + keyword + "%");
            }
            if (cityId != null)
            {
                conditions.Add(" Cart.CityId = @CityId ");
                parameters.Add("CityId", cityId);
            }
            if (districtId != null)
            {
                conditions.Add(" Cart.DistrictId = @DistrictId ");
                parameters.Add("DistrictId", districtId);
            }
            if (wardId != null)
            {
                conditions.Add(" Cart.WardId = @WardId ");
                parameters.Add("WardId", wardId);
            }
            if (fromCreatedDate != null)
            {
                conditions.Add(" Cart.CreatedDate >= @FromCreatedDate ");
                parameters.Add("FromCreatedDate", fromCreatedDate);
            }
            if (toCreatedDate != null)
            {
                conditions.Add(" Cart.CreatedDate <= @ToCreatedDate ");
                parameters.Add("ToCreatedDate", toCreatedDate);
            }
            if (fromTotal != null)
            {
                conditions.Add(" Bill.Total >= @FromTotal ");
                parameters.Add("FromTotal", fromTotal);
            }
            if (toTotal != null)
            {
                conditions.Add(" Bill.Total <= @ToTotal ");
                parameters.Add("ToTotal", toTotal);
            }
            if (paymentMethod != null)
            {
                conditions.Add(" Bill.PaymentMethod = @PaymentMethod ");
                parameters.Add("PaymentMethod", paymentMethod.GetDisplayName());
            }
            if (status != null)
            {
                conditions.Add(" Cart.Status = @Status ");
                parameters.Add("Status", status.GetDisplayName());
            }
            if (conditions.Count > 0)
            {
                where += " WHERE " + string.Join(" AND ", conditions);
            }
            string query = $@" SELECT * 
                                {from} 
                                WHERE Cart.Id IN 
                                            (SELECT DISTINCT(Cart.Id) 
                                            {from} {where} 
                                            ORDER BY Cart.Id OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY)
                                {orderBy}";
            parameters.Add("Skip", (page - 1) * pageSize);
            parameters.Add("Take", pageSize);
            Console.WriteLine(query);
            using (var connection = _dapperContext.CreateConnection())
            {
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
                    )).Distinct().ToList();
                long count = await connection.ExecuteScalarAsync<long>("SELECT COUNT(DISTINCT(Cart.Id)) " + from + where, parameters);
                return new { carts, count };
            }
        }*/


        public async Task<dynamic> GetCarts(string? keywordType, string? keyword,
            string? cityId, string? districtId, string? wardId,
            DateTime? fromCreatedDate, DateTime? toCreatedDate, decimal? fromTotal, decimal? toTotal,
            PaymentMethod? paymentMethod, int page, int pageSize,
            CartStatus? status = CartStatus.PENDING,
            string? sortBy = "CREATEDDATE", string? order = "ASC")
        {
            string where = @"  ";
            List<string> conditions = new List<string>();
            string orderBy = " ";
            if (sortBy == "CREATEDDATE")
                orderBy += " ORDER BY Cart.CreatedDate ";
            else
                orderBy += " ORDER BY Bill.Total ";
            orderBy += order;
            DynamicParameters parameters = new DynamicParameters();
            if (keywordType != null)
            {
                if (keywordType == "NAME")
                {
                    conditions.Add(" Cart.Name LIKE @Keyword ");
                }
                else
                {
                    conditions.Add(" Cart.Phone LIKE @Keyword ");
                }
                parameters.Add("Keyword", "%" + keyword + "%");
            }
            if (cityId != null)
            {
                conditions.Add(" Cart.CityId = @CityId ");
                parameters.Add("CityId", cityId);
            }
            if (districtId != null)
            {
                conditions.Add(" Cart.DistrictId = @DistrictId ");
                parameters.Add("DistrictId", districtId);
            }
            if (wardId != null)
            {
                conditions.Add(" Cart.WardId = @WardId ");
                parameters.Add("WardId", wardId);
            }
            if (fromCreatedDate != null)
            {
                conditions.Add(" Cart.CreatedDate >= @FromCreatedDate ");
                parameters.Add("FromCreatedDate", fromCreatedDate);
            }
            if (toCreatedDate != null)
            {
                conditions.Add(" Cart.CreatedDate <= @ToCreatedDate ");
                parameters.Add("ToCreatedDate", toCreatedDate);
            }
            if (fromTotal != null)
            {
                conditions.Add(" Bill.Total >= @FromTotal ");
                parameters.Add("FromTotal", fromTotal);
            }
            if (toTotal != null)
            {
                conditions.Add(" Bill.Total <= @ToTotal ");
                parameters.Add("ToTotal", toTotal);
            }
            if (paymentMethod != null)
            {
                conditions.Add(" Bill.PaymentMethod = @PaymentMethod ");
                parameters.Add("PaymentMethod", paymentMethod.GetDisplayName());
            }
            if (status != null)
            {
                conditions.Add(" Cart.Status = @Status ");
                parameters.Add("Status", status.GetDisplayName());
            }
            if (conditions.Count > 0)
            {
                where += " WHERE " + string.Join(" AND ", conditions);
            }
            string from = $@" FROM (SELECT Cart.Id, Cart.Phone, Cart.Name, Cart.CityId, Cart.DistrictId, Cart.WardId,
                                                    Cart.DetailLocation, Cart.Status, Cart.CreatedDate, Cart.ProcessDescription, ProcessAccountId,
                                                    Bill.Id as BillId, Bill.PurchaseDate, Bill.Total, Bill.PaymentMethod, Bill.OrderCode
                                                FROM Cart LEFT OUTER JOIN Bill ON Cart.BillId = Bill.Id 
                                                {where}
                                                {orderBy} OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY) as CartBill
                                        JOIN CartDetail ON CartBill.Id = CartDetail.CartId
                                        JOIN Product ON CartDetail.ProductId = Product.Id
                                        JOIN Account ON Account.Id = CartDetail.AccountId
                                        OUTER APPLY(SELECT TOP 1 * FROM ProductImage WHERE ProductImage.ProductId = Product.Id) as img ";
            string query = $@" SELECT  CartBill.Id, CartBill.Phone, CartBill.Name, CartBill.CityId, CartBill.DistrictId, CartBill.WardId,
                                       CartBill.DetailLocation, CartBill.Status, CartBill.CreatedDate, CartBill.ProcessDescription, ProcessAccountId,
                                       CartBill.BillId as Id, CartBill.PurchaseDate, CartBill.Total, CartBill.PaymentMethod, CartBill.OrderCode,
	                                   CartDetail.Id, CartDetail.Price, CartDetail.Status, CartDetail.Unit, CartDetail.Quantity,
	                                   Product.Id, Product.Name, Product.Price, Product.Unit, Product.MinPurchase, Product.Status, Product.Description, Product.Quantity,
	                                   Account.Id, Account.Phone, Account.Email, Account.FirstName, Account.LastName, Account.CityId, Account.DistrictId, Account.WardId,
	                                   img.Id, img.Url, img.FileName
                                {from} ";
            parameters.Add("Skip", (page - 1) * pageSize);
            parameters.Add("Take", pageSize);
            Console.WriteLine(query);
            using (var connection = _dapperContext.CreateConnection())
            {
                var cartDictionary = new Dictionary<long, Cart>();
                IList<Cart> carts = (await connection.QueryAsync<Cart, Bill, CartDetail, Product, Account, ProductImage, Cart>
                    (
                        query,
                        (cart, bill, cartDetail, product, account, productImage) =>
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
                                if (bill != null)
                                {
                                    cartEntry.Bill = bill;
                                    cartEntry.BillId = bill.Id;
                                }
                            }
                            cartEntry.CartDetails.Add(cartDetail);
                            Console.WriteLine($"CHECK {cart.BillId}");
                            return cartEntry;
                        },
                        param: parameters,
                        splitOn: "Id, Id, Id, Id, Id, Id "
                    )).Distinct().ToList();
                long count = await connection.ExecuteScalarAsync<long>($@"SELECT COUNT(DISTINCT(Cart.Id)) 
                                                                            FROM Cart JOIN CartDetail ON Cart.Id = CartDetail.CartId
                                                                                    LEFT OUTER JOIN Bill ON Bill.Id = Cart.BillId
                                                                                    JOIN Product ON CartDetail.ProductId = Product.Id
                                                                                    JOIN Account ON Account.Id = CartDetail.AccountId
                                                                                    OUTER APPLY(SELECT TOP 1 * FROM ProductImage WHERE ProductImage.ProductId = Product.Id) as img 
                                                                            {where}",
                                                                        parameters);
                return new { carts, count };
            }

        }



        public async Task<dynamic> GetUserCartHistory(long accountId, CartStatus? status, string? paymentMethod,
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
                                    LEFT OUTER JOIN Bill ON Bill.Id = Cart.BillId
                                    JOIN Product ON CartDetail.ProductId = Product.Id
                                    JOIN Account ON Account.Id = CartDetail.AccountId
                                    OUTER APPLY (SELECT TOP 1 * FROM ProductImage WHERE ProductImage.ProductId = Product.Id) as img ";
                string where = " WHERE Account.Id = @AccountId ";
                string query = @" SELECT * " + from;
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("AccountId", accountId);
                if (status != null)
                {
                    where += " AND Cart.Status = @Status ";
                    parameters.Add("Status", status.GetDisplayName().ToUpper());
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
                                if (bill != null)
                                {
                                    cartEntry.Bill = bill;
                                    cartEntry.BillId = bill.Id;
                                }
                            }
                            cartEntry.CartDetails.Add(cartDetail);
                            return cartEntry;
                        },
                        param: parameters
                    )).Distinct().Skip((page - 1) * pageSize).Take(pageSize).ToList();
                long count = await connection.ExecuteScalarAsync<long>("SELECT COUNT(DISTINCT(Cart.Id)) " + from + where, parameters);
                return new { carts, count };
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

        public async Task<Cart> ProcessCartById(long id)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @$" SELECT  CartBill.Id, CartBill.Phone, CartBill.Name, CartBill.CityId, CartBill.DistrictId, CartBill.WardId,
                                       CartBill.DetailLocation, CartBill.Status, CartBill.CreatedDate, CartBill.ProcessDescription, ProcessAccountId,
                                       CartBill.BillId as Id, CartBill.PurchaseDate, CartBill.Total, CartBill.PaymentMethod, CartBill.OrderCode,
	                                   CartDetail.Id, CartDetail.Price, CartDetail.Status, CartDetail.Unit, CartDetail.Quantity,
	                                   Product.Id, Product.Name, Product.Price, Product.Unit, Product.MinPurchase, Product.Status, Product.Description, Product.Quantity,
	                                   Account.Id, Account.Phone, Account.Email, Account.FirstName, Account.LastName, Account.CityId, Account.DistrictId, Account.WardId,
	                                   img.Id, img.Url, img.FileName ";
                string from = $@" FROM (SELECT Cart.Id, Cart.Phone, Cart.Name, Cart.CityId, Cart.DistrictId, Cart.WardId,
                                                    Cart.DetailLocation, Cart.Status, Cart.CreatedDate, Cart.ProcessDescription, ProcessAccountId,
                                                    Bill.Id as BillId, Bill.PurchaseDate, Bill.Total, Bill.PaymentMethod, Bill.OrderCode
                                                FROM Cart LEFT OUTER JOIN Bill ON Cart.BillId = Bill.Id) as CartBill
                                        JOIN CartDetail ON CartBill.Id = CartDetail.CartId
                                        JOIN Product ON CartDetail.ProductId = Product.Id
                                        JOIN Account ON Account.Id = CartDetail.AccountId
                                        OUTER APPLY(SELECT TOP 1 * FROM ProductImage WHERE ProductImage.ProductId = Product.Id) as img ";
                string where = " WHERE CartBill.Id = @Id ";
                var cartDictionary = new Dictionary<long, Cart>();
                Cart cart = (await connection.QueryAsync<Cart, Bill, CartDetail, Product, Account, ProductImage, Cart>(query + from + where, (cart, bill, cartDetail, product, account, productImage) =>
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
                        if (bill != null)
                        {
                            cartEntry.Bill = bill;
                            cartEntry.BillId = bill.Id;
                        }
                    }
                    cartEntry.CartDetails.Add(cartDetail);
                    Console.WriteLine($"CHECK {cart.BillId}");
                    return cartEntry;
                }, param: new { Id = id },
                splitOn: "Id, Id, Id, Id, Id, Id ")).Distinct().SingleOrDefault();
                return cart;
            }
        }

        public async Task<Cart> UpdateCartStatus(long id, CartStatus status)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"UPDATE Cart
                                SET Cart.Status = @Status
                                OUTPUT inserted.*
                                WHERE Cart.Id = @Id";
                Cart cart = await connection.QuerySingleOrDefaultAsync<Cart>(query, new { Status = status.GetDisplayName(), Id = id });
                return cart;
            }
        }
    }
}

