using tlcn_dotnet.Entity;

namespace tlcn_dotnet.IRepositories
{
    public interface IProductPromotionRepository
    {
        Task<ProductPromotion> GetPromotionByProductId(long productId);
        Task<Dictionary<long, ProductPromotion>> GetListPromotionFromListProductId(IList<long> listProductId);
    }
}
