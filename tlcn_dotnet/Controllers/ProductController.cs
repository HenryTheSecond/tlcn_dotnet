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
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost]
        public async Task<DataResponse> AddProduct()
        {
            var files = HttpContext.Request.Form.Files;
            var product = HttpContext.Request.Form["product"][0];

            AddProductDto addProductDto = JsonConvert.DeserializeObject<AddProductDto>(product);
            if (!this.TryValidateModel(addProductDto, "ValidateAddProductDto"))
            {
                throw new GeneralException(ModelState["ValidateAddProductDto"].Errors[0].ErrorMessage, ApplicationConstant.BAD_REQUEST_CODE);
            }
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
            if (!this.TryValidateModel(editProductDto, "ValidateEditProductDto"))
            {
                throw new GeneralException(ModelState["ValidateEditProductDto"].Errors[0].ErrorMessage, ApplicationConstant.BAD_REQUEST_CODE);
            }
            return await _productService.EditProduct(id.Value, editProductDto, files);
        }

        [HttpGet]
        public async Task<DataResponse> FilterProduct(string? keyword, string? minPrice, string? maxPrice,
            string? categoryId, string? orderBy, string? order, string? page = "1")
        {
            int numberPage = 1;
            decimal? numberMinPrice = null;
            decimal? numberMaxPrice = null;
            long? numberCategoryId = null;
            ProductOrderBy? productOrderBy = null;
            SortOrder? sortOrder = null;
            try
            {
                numberPage = Int32.Parse(page);
                numberPage = numberPage > 0 ? numberPage : 1;
                numberMinPrice = minPrice == null ? null : Convert.ToDecimal(minPrice);
                numberMaxPrice = maxPrice == null ? null : Convert.ToDecimal(maxPrice);
                numberCategoryId = categoryId == null ? null : Convert.ToInt64(categoryId);

                productOrderBy = Util.ConvertStringToDataType<ProductOrderBy>(orderBy, ProductOrderBy.ID);
                sortOrder = Util.ConvertStringToDataType<SortOrder>(order, SortOrder.DESC);
                Console.WriteLine(productOrderBy); Console.WriteLine(sortOrder);
            }
            catch (Exception e) when (e is ArgumentNullException || e is FormatException || e is ArgumentException)
            {
                throw new GeneralException(ApplicationConstant.BAD_REQUEST, ApplicationConstant.BAD_REQUEST_CODE);
            }
            Console.WriteLine("MIN PRICE " + minPrice);
            Console.WriteLine("MIN PRICE PARSED" + numberMinPrice);
            return await _productService.FilterProduct(keyword, numberMinPrice, numberMaxPrice, 
                numberCategoryId, productOrderBy, sortOrder, numberPage);
        }

        [HttpDelete("{strId}")]
        public async Task<DataResponse> DeleteProduct(string strId)
        {
            long? id = Util.ParseId(strId);
            if (id == null) throw new GeneralException(ApplicationConstant.INVALID_ID, ApplicationConstant.BAD_REQUEST_CODE);

            return await _productService.DeleteProduct(id);
        }

        [HttpGet("GetAllProductIdAndNam")]
        public async Task<DataResponse> GetAllProductIdAndName()
        {
            return await _productService.GetAllProductIdAndName();
        }
    }
}
