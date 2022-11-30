using AutoMapper;
using System.Data.SqlClient;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.AccountDto;
using tlcn_dotnet.Dto.ReviewDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ReviewService(IReviewRepository reviewRepository, IMapper mapper, IProductRepository productRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public async Task<DataResponse> GetAllProductReview(long productId, int page = 1, int pageSize = 5)
        {
            var reviews = await _reviewRepository.GetAllProductReview(productId, page, pageSize);
            var count = await _reviewRepository.CountProductReview(productId);
            var reviewResponses = _mapper.Map<IEnumerable<ReviewResponse>>(reviews);
            return new DataResponse(new 
            {
                reviews = reviewResponses,
                maxPage = Util.CalculateMaxPage(count, pageSize),
                currentPage = page
            });
        }
        public async Task<DataResponse> ReviewProduct(string authorization, long productId, ReviewRequest reviewRequest)
        {
            /*Product product = await _productRepository.GetById(productId)
                ?? throw new GeneralException("PRODUCT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);*/
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);

            long id = 0;
            try
            {
                id = await _reviewRepository.InsertReview(accountId, productId, reviewRequest);
            }
            catch (SqlException e)
            {
                if (e.Message.Contains("duplicate"))
                    throw new GeneralException("YOU HAVE ALREADY REVIEWED", ApplicationConstant.FAILED_CODE);
                throw e;
            }
            AccountReviewDto accountReviewDto = _mapper.Map<AccountReviewDto>(Util.ReadJwtTokenAndParseToAccount(authorization));
            ReviewResponse reviewResponse = new ReviewResponse()
            { 
                Id = id,
                Account = accountReviewDto,
                Content = reviewRequest.Content,
                PostDate = DateTime.Now,
                ProductId = productId,
                Rating = reviewRequest.Rating
            };
            return new DataResponse(reviewResponse);
        }
    }
}
