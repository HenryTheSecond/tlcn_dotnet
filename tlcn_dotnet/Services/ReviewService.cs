﻿using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
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
        private readonly MyDbContext _dbContext;
        public ReviewService(IReviewRepository reviewRepository, IMapper mapper, IProductRepository productRepository, MyDbContext dbContext)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _dbContext = dbContext;
        }

        public async Task<DataResponse> AdminDeleteReview(long id)
        {
            var review = await _dbContext.Review.FindAsync(id);
            if (review == null)
                throw new GeneralException("REVIEW NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            _dbContext.Review.Remove(review);
            int rowAffected = await _dbContext.SaveChangesAsync();
            return new DataResponse(rowAffected > 0 ? true : false);
        }

        public async Task<DataResponse> DeleteReview(string authorization, long id)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            int affectedRow = await _reviewRepository.DeleteReview(id, accountId);
            if (affectedRow == 0)
                throw new GeneralException("REVIEW NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            return new DataResponse(true);
        }

        public async Task<DataResponse> EditReview(string authorization, long id, ReviewRequest reviewRequest)
        {
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            Review review = await _reviewRepository.EditReview(accountId, id, reviewRequest)
                ?? throw new GeneralException("REVIEW NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);

            review.Account = Util.ReadJwtTokenAndParseToAccount(authorization);
            return new DataResponse(_mapper.Map<ReviewResponse>(review));
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

        public async Task<DataResponse> GetUserReviewOfProduct(string authorization, long productId)
        {
            Product product = await _productRepository.GetById(productId)
                ?? throw new GeneralException("PRODUCT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);
            Review review = _dbContext.Review.Include(review => review.ReviewResource).Where(review => review.Account.Id == accountId && review.Product.Id == productId).SingleOrDefault();
            return new DataResponse(_mapper.Map<ReviewResponse>(review));
        }

        public async Task<DataResponse> ReviewProduct(string authorization, long productId, ReviewRequest reviewRequest)
        {
            Product product = await _productRepository.GetById(productId)
                ?? throw new GeneralException("PRODUCT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            long accountId = Util.ReadJwtTokenAndGetAccountId(authorization);

            if (!(await _productRepository.CheckAccountBuyItem(accountId, productId)))
                throw new GeneralException("Bạn chưa thể bình luận nếu chưa mua sản phẩm", ApplicationConstant.BAD_REQUEST_CODE);

            long id = 0;
            try
            {
                id = await _reviewRepository.InsertReview(accountId, productId, reviewRequest);
            }
            catch (SqlException e)
            {
                if (e.Message.Contains("duplicate"))
                    throw new GeneralException("Bạn đã bình luận ở sản phẩm này", ApplicationConstant.FAILED_CODE);
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

        public async Task<DataResponse> SearchReview(string keyword, long? productId, int page, int pageSize)
        {
            var query = from review in _dbContext.Review
                        join account in _dbContext.Account on review.AccountId equals account.Id
                        join googleAccount in _dbContext.GoogleAccount on account.Id equals googleAccount.Account.Id into grpGGAccount
                        from googleAccount in grpGGAccount.DefaultIfEmpty()
                        where (review.Content.Contains(keyword) ||
                                account.Email.Contains(keyword) ||
                                googleAccount.Email.Contains(keyword) ||
                                (account.FirstName + " " + account.LastName).Contains(keyword))
                        select new { Review = review, Account = account, GoogleAccount = googleAccount };
            if(productId != null)
                query = query.Where(review => review.Review.ProductId == productId);

            int count = await query.CountAsync();
            var resultQuery = await (from row in query.Skip((page - 1) * pageSize).Take(pageSize)
                              join reviewResource in _dbContext.ReviewResource on row.Review.Id equals reviewResource.ReviewId into grpReviewResource
                              from reviewResource in grpReviewResource.DefaultIfEmpty()
                              select new { Row = row, ReviewResource = reviewResource}).ToListAsync();
            Dictionary<long, Review> dictReview = new Dictionary<long, Review>();
            resultQuery.ForEach(item =>
            {
                /*row.Review.Account = row.Account;
                row.Review.AccountId = row.Account.Id.Value;*/
                if(!dictReview.ContainsKey(item.Row.Review.Id.Value))
                {
                    item.Row.Review.Account = item.Row.Account;
                    item.Row.Review.AccountId = item.Row.Account.Id.Value;
                    item.Row.Review.ReviewResource = new List<ReviewResource>();
                    dictReview.Add(item.Row.Review.Id.Value, item.Row.Review);
                }
                if (item.ReviewResource != null)
                    dictReview[item.Row.Review.Id.Value].ReviewResource.Add(item.ReviewResource);
            });

            var result = dictReview.Values.ToList();
            return new DataResponse(new
            {
                reviews = _mapper.Map<IEnumerable<ReviewResponse>>(result),
                maxPage = Util.CalculateMaxPage(count, pageSize),
                currentPage = page
            });

        }
    }
}
