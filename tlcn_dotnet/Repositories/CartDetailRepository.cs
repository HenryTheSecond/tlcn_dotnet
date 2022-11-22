using CloudinaryDotNet;
using Dapper;
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
        private readonly string CART_DETAIL_SELECT = @"SELECT CartDetail.Id, CartDetail.Price, CartDetail.Status, CartDetail.Unit, CartDetail.Quantity,
		                                                        Product.Id, Product.Name, Product.Price, Product.Quantity, Product.Unit, Product.MinPurchase, Product.Status, Product.Description,
		                                                        Category.Id, Category.Name,
		                                                        Image.Id, Image.Url, Image.FileName
                                                        FROM ((CartDetail left outer join Product on CartDetail.ProductId = Product.Id)
		                                                        left outer join Category on Product.CategoryId = Category.Id)
		                                                        outer apply (SELECT TOP 1 ProductImage.Id, ProductImage.FileName, ProductImage.Url from ProductImage where ProductImage.ProductId = Product.Id) as Image ";

        private readonly DapperContext _dapperContext;
        public CartDetailRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }
        public async Task<CartDetail> AddCartDetail(dynamic parameters)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"INSERT INTO CartDetail(Status, Unit, Quantity, ProductId, AccountId)
                                    OUTPUT INSERTED.Id
                                    VALUES (@Status, @Unit, @Quantity, @ProductId, @AccountId)";
                long id = await connection.ExecuteScalarAsync<long>(query, new DynamicParameters(parameters));
                return await GetById(id);
            }
        }

        public async Task<long> CheckCurrentCartHavingProduct(long accountId, long productId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"SELECT CartDetail.Id 
                                FROM CartDetail 
                                WHERE AccountId = @AccountId AND ProductId = @ProductId AND CartId IS NULL";

                long id = await connection.ExecuteScalarAsync<long>(query, new
                {
                    AccountId = accountId,
                    ProductId = productId
                });
                return id;
            }
        }

        public async Task<int> DeleteCartDetailByIdAndAccountId(long id, long accountId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"DELETE FROM CartDetail
                                WHERE Id = @Id AND AccountId = AccountId AND CartId IS NULL";
                return await connection.ExecuteAsync(query, new
                {
                    Id = id,
                    AccountId = accountId
                });
            }
        }

        public async Task<CartDetail> GetById(long id)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = CART_DETAIL_SELECT + " WHERE CartDetail.Id = @Id ";
                var cartDetails = await connection.QueryAsync<CartDetail, Product, Category, ProductImage, CartDetail>(query,
                    (cartDetail, product, category, productImage) =>
                    {
                        if (productImage != null)
                            product.ProductImages.Add(productImage);
                        product.Category = category;
                        cartDetail.Product = product;
                        cartDetail.ProductId = product.Id;
                        return cartDetail;
                    }, splitOn: "Id",
                    param: new { Id = id});
                return cartDetails.SingleOrDefault() ??
                    throw new GeneralException("CART DETAIL NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            }
        }

        public async Task<IEnumerable<CartDetail>> GetCurrentCart(long accountId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = CART_DETAIL_SELECT + " WHERE AccountId = @AccountId AND CartId IS NULL";
                var cartDetails = await connection.QueryAsync<CartDetail, Product, Category, ProductImage, CartDetail>(query,
                    (cartDetail, product, category, productImage) =>
                    {
                        if (productImage != null)
                            product.ProductImages.Add(productImage);
                        product.Category = category;
                        cartDetail.Product = product;
                        cartDetail.ProductId = product.Id;
                        return cartDetail;
                    }, splitOn: "Id",
                    param: new { AccountId = accountId});
                return cartDetails;

            }
        }

        public async Task<IEnumerable<CartDetail>> GetListCart(long accountId, IList<long> listCartDetailId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = CART_DETAIL_SELECT +
                    " WHERE CartDetail.Id in @ListId AND CartDetail.AccountId = @AccountId AND CartDetail.CartId IS NULL";
                var cartDetails = await connection.QueryAsync<CartDetail, Product, Category, ProductImage, CartDetail>(query,
                    (cartDetail, product, category, productImage) =>
                    {
                        if (productImage != null)
                            product.ProductImages.Add(productImage);
                        product.Category = category;
                        cartDetail.Product = product;
                        cartDetail.ProductId = product.Id;
                        return cartDetail;
                    }, splitOn: "Id",
                    param: new 
                    {
                        ListId = listCartDetailId,
                        AccountId = accountId
                    });
                return cartDetails;
            }
        }

        public async Task<int> InsertPriceAndCartId(long id, decimal price, long cartId)
        {
            using (var connection = _dapperContext.CreateConnection())
            { 
                string query = @"UPDATE CartDetail 
                                SET Price = @Price, CartId = @CartId
                                WHERE Id = @Id";
                return await connection.ExecuteAsync(query, new
                {
                    Price = price,
                    CartId = cartId,
                    Id = id
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
    }
}
