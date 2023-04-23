using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tlcn_dotnet.Dto.ProductPromotionDto;
using tlcn_dotnet.IServices;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductPromotionController : ControllerBase
    {
        private readonly IProductPromotionService _productPromotionService;
        public ProductPromotionController(IProductPromotionService productPromotionService)
        {
            _productPromotionService = productPromotionService;
        }

        [HttpPost("addProductPromotion")]
        public async Task<DataResponse> AddProductPromotion([FromBody] ProductPromotionAddRequest request)
        {
            return await _productPromotionService.AddProductPromotion(request);
        }

        [HttpPut("updateProductPromotion/{id}")]
        public async Task<DataResponse> UpdateProductPromotion(long id,[FromBody] ProductPromotionUpdateRequest request)
        {
            return await _productPromotionService.UpdateProductPromotion(id, request);
        }
    }
}
