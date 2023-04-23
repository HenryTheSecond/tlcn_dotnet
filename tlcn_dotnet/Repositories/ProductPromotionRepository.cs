using Irony.Parsing;
using Microsoft.EntityFrameworkCore;
using tlcn_dotnet.Entity;
using tlcn_dotnet.IRepositories;

namespace tlcn_dotnet.Repositories
{
    public class ProductPromotionRepository : IProductPromotionRepository
    {
        private readonly MyDbContext _dbContext;
        public ProductPromotionRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Dictionary<long, ProductPromotion>> GetListPromotionFromListProductId(IList<long> listProductId)
        {
            return await _dbContext.ProductPromotion.Where(promotion => listProductId.Contains(promotion.ProductId) && promotion.ExpireDate > DateTime.Now && promotion.IsEnable == true).ToDictionaryAsync(p => p.ProductId);
        }

        public async Task<ProductPromotion> GetPromotionByProductId(long productId)
        {
            return await _dbContext.ProductPromotion.FirstOrDefaultAsync(promotion => promotion.ProductId == productId && promotion.ExpireDate > DateTime.Now && promotion.IsEnable == true);
        }
    }
}
