using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
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
        public async Task<DataResponse> EditProduct(string strId)
        {
            long? id = Util.ParseId(strId);
            if(id == null) throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);
            var files = HttpContext.Request.Form.Files;
            var strEditProduct = HttpContext.Request.Form["editProduct"][0];

            EditProductDto editProductDto = JsonConvert.DeserializeObject<EditProductDto>(strEditProduct);
            return await _productService.EditProduct(id.Value, editProductDto, files);
        }

        [HttpGet]
        public async Task<DataResponse> FilterProduct(string? keyword, string? minPrice, string? maxPrice,
            string? categoryId, string? page = "1")
        {
            int numberPage = 1;
            decimal? numberMinPrice = null;
            decimal? numberMaxPrice = null;
            long? numberCategoryId = null;
            try
            {
                numberPage = Int32.Parse(page);
                numberPage = numberPage > 0 ? numberPage : 1;
                numberMinPrice = minPrice == null ? null : Convert.ToDecimal(minPrice);
                numberMaxPrice = maxPrice == null ? null : Convert.ToDecimal(maxPrice);
                numberCategoryId = categoryId == null ? null : Convert.ToInt64(categoryId);
            }
            catch (Exception e) when (e is ArgumentNullException || e is FormatException)
            {
                throw new GeneralException(ApplicationConstant.BAD_REQUEST, ApplicationConstant.BAD_REQUEST_CODE);
            }
            Console.WriteLine("MIN PRICE " + minPrice);
            Console.WriteLine("MIN PRICE PARSED" + numberMinPrice);
            return await _productService.FilterProduct(keyword, numberMinPrice, numberMaxPrice, numberCategoryId, numberPage);
        }

        [HttpDelete("{strId}")]
        public async Task<DataResponse> DeleteProduct(string strId)
        {
            long? id = Util.ParseId(strId);
            if (id == null) throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);

            return await _productService.DeleteProduct(id);
        }
    }
}
