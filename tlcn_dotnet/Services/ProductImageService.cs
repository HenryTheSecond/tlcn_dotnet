using tlcn_dotnet.Constant;
using tlcn_dotnet.Dto.ProductImageDto;
using tlcn_dotnet.Entity;

namespace tlcn_dotnet.Services
{
    public interface ProductImageService
    {
        public Task<IEnumerable<SimpleProductImageDto>> AddProductImages(IFormFileCollection images, Product product);
        public Task<IEnumerable<SimpleProductImageDto>> GetImageByProduct(long? productId);
        public Task<IEnumerable<SimpleProductImageDto>> EditProductImage(Product product, IList<ProductImageEditStatus> editStatus, IFormFileCollection files);

        public Task DeleteAllImageOfProduct(long? id);
    }
}
