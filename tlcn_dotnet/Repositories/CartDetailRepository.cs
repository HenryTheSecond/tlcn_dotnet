using CloudinaryDotNet;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Dto.CartDetailDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class CartDetailRepository : ICartDetailRepository
    {
        private readonly string CART_DETAIL_SELECT = @"SELECT CartDetail.Id, CartDetail.Price, CartDetail.Status, CartDetail.Unit, CartDetail.Quantity, CartDetail.GiftCartId,
		                                                        Product.Id, Product.Name, Product.Price, Product.Unit, Product.MinPurchase, Product.Status, Product.Description, Product.Quantity, Product.IsDeleted,
		                                                        Category.Id, Category.Name,
		                                                        Image.Id, Image.Url, Image.FileName,
                                                                GiftCart.Id, GiftCart.Name, GiftCart.IsActive, GiftCart.AccountId
                                                        FROM ((CartDetail left outer join Product on CartDetail.ProductId = Product.Id)
		                                                        left outer join Category on Product.CategoryId = Category.Id)
		                                                        outer apply (SELECT TOP 1 ProductImage.Id, ProductImage.FileName, ProductImage.Url from ProductImage where ProductImage.ProductId = Product.Id) as Image 
                                                                LEFT OUTER JOIN GiftCart ON CartDetail.GiftCartId = GiftCart.Id";

        private readonly DapperContext _dapperContext;
        private readonly MyDbContext _dbContext;
        public CartDetailRepository(DapperContext dapperContext, MyDbContext dbContext)
        {
            _dapperContext = dapperContext;
            _dbContext = dbContext;
        }
        public async Task<CartDetail> AddCartDetail(dynamic parameters)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                /*string query = @"INSERT INTO CartDetail(Status, Unit, Quantity, ProductId, AccountId)
                                    OUTPUT INSERTED.Id
                                    VALUES (@Status, @Unit, @Quantity, @ProductId, @AccountId)";
                long id = await connection.ExecuteScalarAsync<long>(query, new DynamicParameters(parameters));*/

                string procedure = "sp_InsertCartDetail";
                long id = await connection.ExecuteScalarAsync<long>(procedure, new DynamicParameters(parameters), commandType: CommandType.StoredProcedure);
                return await GetById(id);
            }
        }

        public async Task<long> CheckCurrentCartHavingProduct(long accountId, long productId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string procedure = @"sp_CheckCurrentCartHavingProduct";

                long id = await connection.ExecuteScalarAsync<long>(procedure, new
                {
                    AccountId = accountId,
                    ProductId = productId
                },
                commandType: CommandType.StoredProcedure);
                return id;
            }
        }

        public async Task<int> DeleteCartDetailByIdAndAccountId(long id, long accountId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string procedure = "sp_DeleteCartDetailByIdAndAccountId";
                return await connection.ExecuteAsync(procedure, new
                {
                    Id = id,
                    AccountId = accountId
                },
                commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<CartDetail> GetById(long id)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = CART_DETAIL_SELECT + " WHERE CartDetail.Id = @Id ";
                var cartDetails = await connection.QueryAsync<CartDetail, Product, Category, ProductImage, GiftCart, CartDetail>(query,
                    (cartDetail, product, category, productImage, giftCart) =>
                    {
                        if (productImage != null)
                            product.ProductImages.Add(productImage);
                        product.Category = category;
                        cartDetail.Product = product;
                        cartDetail.ProductId = product.Id;
                        cartDetail.GiftCart = giftCart;
                        return cartDetail;
                    }, splitOn: "Id",
                    param: new { Id = id});
                return cartDetails.SingleOrDefault() ??
                    throw new GeneralException("CART DETAIL NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            }
        }

        public async Task<IList<CartDetail>> GetCurrentCart(long accountId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = CART_DETAIL_SELECT + " WHERE CartDetail.AccountId = @AccountId AND CartDetail.CartId IS NULL";
                var cartDetails = await connection.QueryAsync<CartDetail, Product, Category, ProductImage, GiftCart, CartDetail>(query,
                    (cartDetail, product, category, productImage, giftCart) =>
                    {
                        if (productImage != null)
                            product.ProductImages.Add(productImage);
                        product.Category = category;
                        cartDetail.Product = product;
                        cartDetail.ProductId = product.Id;
                        cartDetail.GiftCart = giftCart;
                        return cartDetail;
                    }, splitOn: "Id",
                    param: new { AccountId = accountId});
                return cartDetails.ToList();

            }
        }

        public async Task<IList<CartDetail>> GetListCart(long accountId, IList<long> listCartDetailId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = CART_DETAIL_SELECT +
                    " WHERE CartDetail.Id in @ListId AND CartDetail.AccountId = @AccountId AND CartDetail.CartId IS NULL";
                var cartDetails = await connection.QueryAsync<CartDetail, Product, Category, ProductImage, GiftCart, CartDetail>(query,
                    (cartDetail, product, category, productImage, giftCart) =>
                    {
                        if (productImage != null)
                            product.ProductImages.Add(productImage);
                        product.Category = category;
                        cartDetail.Product = product;
                        cartDetail.ProductId = product.Id;
                        cartDetail.GiftCart = giftCart;
                        return cartDetail;
                    }, splitOn: "Id",
                    param: new 
                    {
                        ListId = listCartDetailId,
                        AccountId = accountId
                    });
                return cartDetails.ToList();
            }
        }

        public async Task<int> UpdatePriceAndCartId(long id, decimal price, long cartId)
        {
            using (var connection = _dapperContext.CreateConnection())
            { 
                string query = @"UPDATE CartDetail 
                                SET Price = @Price, CartId = @CartId, Status = @Status
                                WHERE Id = @Id";
                return await connection.ExecuteAsync(query, new
                {
                    Price = price,
                    CartId = cartId,
                    Id = id,
                    Status = CartDetailStatus.PAID.GetDisplayName()
                });
            }
        }

        public async Task<CartDetail> UpdateCartDetailQuantity(long id, double quantity)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"UPDATE CartDetail 
                                SET Quantity = @Quantity 
                                WHERE Id = @Id";
                await connection.ExecuteAsync(query, new
                {
                    Quantity = quantity,
                    Id = id
                });
                return await GetById(id);
            }
        }

        public async Task<CartDetail> UpdateCartDetailQuantity(long productId, double quantity, long accountId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"UPDATE CartDetail 
                                SET Quantity = @Quantity 
                                OUTPUT inserted.Id
                                WHERE ProductId = @ProductId AND AccountId = @AccountId AND CartId IS NULL";
                long id = await connection.ExecuteScalarAsync<long>(query, new
                {
                    Quantity = quantity,
                    ProductId = productId,
                    AccountId = accountId
                });
                if (id == 0)
                    return null;
                return await GetById(id);
            }
        }

        public async Task DeleteCartDetailHavingDeletedProductByAccountId(long accountId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                await connection.ExecuteAsync(@"DELETE CartDetail 
                                                FROM CartDetail JOIN Product ON CartDetail.ProductId = Product.Id 
                                                WHERE CartId IS NULL AND Product.IsDeleted = 1 AND CartDetail.AccountId = @AccountId",
                                            new { AccountId = accountId });
            }
        }

        public async Task<long> CheckCurrentCartHavingProduct(long accountId, long productId, long? giftCartId)
        {
            var cartDetail = await  _dbContext.CartDetail
                .Where(cartDetail => cartDetail.Account.Id == accountId && cartDetail.ProductId == productId && cartDetail.CartId == null && cartDetail.GiftCartId == giftCartId).FirstOrDefaultAsync();
            if (cartDetail == null)
                return 0;
            return cartDetail.Id.Value;
        }

        public async Task<CartDetail> AddCartDetail(CartDetail cartDetail)
        {
            await _dbContext.CartDetail.AddAsync(cartDetail);
            await _dbContext.SaveChangesAsync();
            return await GetById(cartDetail.Id.Value);
        }

        public async Task<CartDetail> FindByIdAndAccountId(long id, long accountId)
        {
            CartDetail cartDetail = await _dbContext.CartDetail.Include(cd => cd.Product)
                .FirstOrDefaultAsync(cd => cd.Id == id && cd.AccountId == accountId);
            return cartDetail;
        }
    }
}
