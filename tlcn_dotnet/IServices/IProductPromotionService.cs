using tlcn_dotnet.Dto.ProductPromotionDto;

namespace tlcn_dotnet.IServices
{
    public interface IProductPromotionService
    {
        Task<DataResponse> AddProductPromotion(ProductPromotionAddRequest request);
        Task<DataResponse> UpdateProductPromotion(long id, ProductPromotionUpdateRequest request);
    }
}
