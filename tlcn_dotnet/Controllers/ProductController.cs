using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
using tlcn_dotnet.Constant;
using tlcn_dotnet.CustomException;
using tlcn_dotnet.Dto.ProductDto;
using tlcn_dotnet.Services;
using tlcn_dotnet.Utils;

namespace tlcn_dotnet.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }
        [HttpPost]
        public async Task<DataResponse> AddProduct()
        {
            var files = HttpContext.Request.Form.Files;
            var product = HttpContext.Request.Form["product"][0];

            AddProductDto addProductDto = JsonConvert.DeserializeObject<AddProductDto>(product);
            return await _productService.AddProduct(addProductDto, files);
        }

        [HttpGet("{strId}")]
        public async Task<DataResponse> GetProductById(string strId)
        {
            long? id = Util.ParseId(strId);
            if (id == null) throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            return await _productService.GetProductById(id);
        }

        [HttpPut("{strId}")]
        public async Task<DataResponse> EditProduct()
        {
            return null;
        }
    }
}
