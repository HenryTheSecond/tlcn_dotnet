using Microsoft.AspNetCore.Http;
using tlcn_dotnet.Dto.ProductDto;

namespace tlcn_dotnet.Services
{
    public interface ProductService
    {
        public Task<DataResponse> AddProduct(AddProductDto addProductDto, IFormFileCollection images);
        public Task<DataResponse> GetProductById(long? id);
    }
}
