using Microsoft.AspNetCore.Http;
using tlcn_dotnet.Dto.ProductDto;

namespace tlcn_dotnet.Services
{
    public interface IProductService
    {
        public Task<DataResponse> AddProduct(AddProductDto addProductDto, IFormFileCollection images);
        public Task<DataResponse> GetProductById(long? id);
        public Task<DataResponse> EditProduct(long? id, EditProductDto editProductDto, IFormFileCollection files);
        public Task<DataResponse> FilterProduct(string? keyword, decimal? minPrice, decimal? maxPrice, long? categoryId, int page);
        public Task<DataResponse> DeleteProduct(long? id);
    }
}
