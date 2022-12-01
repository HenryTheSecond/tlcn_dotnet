using Dapper;
using System.Data;
using tlcn_dotnet.DatabaseContext;
using tlcn_dotnet.Dto.ReviewDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{

    public class ReviewRepository : IReviewRepository
    {
        private readonly DapperContext _dapperContext;
        public ReviewRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<long> CountProductReview(long productId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string function = "dbo.CountProductReview";
                string query = $"SELECT {function}(@ProductId) ";
                long count = await connection.ExecuteScalarAsync<long>(query, new { ProductId = productId });
                return count;
            }
        }

        public async Task<int> DeleteReview(long id, long accountId)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string procedure = "sp_DeleteReview";
                int affectedRow = await connection.ExecuteAsync(procedure,
                    new { Id = id, AccountId = accountId }, commandType: CommandType.StoredProcedure);
                return affectedRow;
            }
        }

        public async Task<Review> EditReview(long accountId, long id, ReviewRequest reviewRequest)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string procedure = "sp_EditReview";
                DynamicParameters parameters = new DynamicParameters(reviewRequest);
                parameters.Add("Id", id);
                parameters.Add("AccountId", accountId);
                var review = await connection.QuerySingleOrDefaultAsync<Review>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return review;
            }
        }

        public async Task<IEnumerable<Review>> GetAllProductReview(long productId, int page = 1, int pageSize = 5)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string procedure = "sp_GetProductReviewPaging";
                DynamicParameters parameters = new DynamicParameters(new
                {
                    ProductId = productId,
                    Page = page,
                    PageSize = pageSize,
                });
                var reviews = await connection.QueryAsync<Review, Account, Review>(procedure, 
                                            (review, account) => 
                                            {
                                                review.Account = account;
                                                return review;
                                            }, param: parameters,
                                            commandType: CommandType.StoredProcedure);
                return reviews;
            }
        }

        public async Task<long> InsertReview(long accountId, long productId, ReviewRequest reviewRequest)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string procedure = "sp_InsertReview";
                DynamicParameters parameters = new DynamicParameters(reviewRequest);
                parameters.Add("AccountId", accountId);
                parameters.Add("PostDate", DateTime.Now);
                parameters.Add("ProductId", productId);

                long id = await connection.ExecuteScalarAsync<long>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return id;
            }
        }

    }
}
