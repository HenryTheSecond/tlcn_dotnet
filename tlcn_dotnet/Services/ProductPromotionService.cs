using AutoMapper;
using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.ProductPromotionDto;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;
using tlcn_dotnet.IServices;

namespace tlcn_dotnet.Services
{
    public class ProductPromotionService : IProductPromotionService
    {
        private readonly MyDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IProductPromotionRepository _productPromotionRepository;
        public ProductPromotionService(MyDbContext dbContext, IMapper mapper, IProductPromotionRepository productPromotionRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _productPromotionRepository = productPromotionRepository;
        }

        public async Task<DataResponse> AddProductPromotion(ProductPromotionAddRequest request)
        {
            var existedPromotion = await _dbContext.ProductPromotion.AnyAsync(promotion => promotion.ProductId == request.ProductId && promotion.ExpireDate > DateTime.Now && promotion.IsEnable == true);
            if (existedPromotion == true)
                throw new GeneralException("PRODUCT IS HAVING A PROMOTION", ApplicationConstant.BAD_REQUEST_CODE);
            Product product = await _dbContext.Product.FirstOrDefaultAsync(product => product.Id == request.ProductId)
                ?? throw new GeneralException("PRODUCT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            if (request.Type == ProductPromotionType.PERCENTAGE && request.Value < 0 && request.Value > 100)
                throw new GeneralException("PROMOTION PERCENTAGE IS INVALID", ApplicationConstant.BAD_REQUEST_CODE);
            if(request.Type == ProductPromotionType.PRICE && request.Value > product.Price)
                throw new GeneralException("PROMOTION PRICE IS INVALID", ApplicationConstant.BAD_REQUEST_CODE);
            ProductPromotion promotion = _mapper.Map<ProductPromotion>(request);
            await _dbContext.ProductPromotion.AddAsync(promotion);
            await _dbContext.SaveChangesAsync();
            promotion.Product = product;
            return new DataResponse(_mapper.Map<ProductPromotionResponse>(promotion));
        }

        public async Task<DataResponse> GetPromotionByProductId(long productId)
        {
            bool isProductExisted = await _dbContext.Product.AnyAsync(p => p.Id == productId);
            if (isProductExisted == false)
                throw new GeneralException("PRODUCT NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            var promotion = await _productPromotionRepository.GetPromotionByProductId(productId);
            return new DataResponse(_mapper.Map<SimpleProductPromotionDto>(promotion));
        }

        public async Task<DataResponse> UpdateProductPromotion(long id, ProductPromotionUpdateRequest request)
        {
            ProductPromotion promotion = await _dbContext.ProductPromotion
                .Include(p => p.Product)
                .FirstOrDefaultAsync(promotion => promotion.Id == id) ??
                throw new GeneralException("PROMOTION NOT FOUND", ApplicationConstant.NOT_FOUND_CODE);
            
            promotion.Type = request.Type != null ? request.Type.Value : promotion.Type;
            promotion.Value = request.Value != null ? request.Value.Value : promotion.Value;
            promotion.ExpireDate = request.ExpireDate != null ? request.ExpireDate.Value : promotion.ExpireDate;
            promotion.IsEnable = request.IsEnable != null ? request.IsEnable.Value : promotion.IsEnable;

            if (promotion.Type == ProductPromotionType.PERCENTAGE && promotion.Value < 0 && promotion.Value > 100)
                throw new GeneralException("PROMOTION PERCENTAGE IS INVALID", ApplicationConstant.BAD_REQUEST_CODE);
            if (promotion.Type == ProductPromotionType.PRICE && promotion.Value > promotion.Product.Price)
                throw new GeneralException("PROMOTION PRICE IS INVALID", ApplicationConstant.BAD_REQUEST_CODE);
            if(promotion.ExpireDate >= DateTime.Now && promotion.IsEnable == true)
            {
                bool currentValidPromotion = await _dbContext.ProductPromotion
                    .AnyAsync(p => p.ExpireDate >= DateTime.Now && p.IsEnable == true && p.ProductId == promotion.ProductId && p.Id != id);
                if (currentValidPromotion == true)
                    throw new GeneralException("PRODUCT IS HAVING PROMOTION", ApplicationConstant.BAD_REQUEST_CODE);
            }
            await _dbContext.SaveChangesAsync();
            return new DataResponse(_mapper.Map<ProductPromotionResponse>(promotion));
        }
    }
}
